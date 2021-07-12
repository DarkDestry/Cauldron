using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Core;
using Cauldron.Primitives;
using SharpGL.SceneGraph;

//using Color = Microsoft.Xna.Framework.Color;

namespace Cauldron.CustomControls
{
	[TemplatePart(Name = "ContextMenu_NewSphere", Type = typeof(MenuItem))]
	[TemplatePart(Name = "HierarchyList", Type = typeof(ListBox))]
	public class Control_HierarchyWindow : Control
	{

		static Control_HierarchyWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_HierarchyWindow), new FrameworkPropertyMetadata(typeof(Control_HierarchyWindow)));
		}

		private ListBox hierarchyListBox;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (Template != null)
			{
				MenuItem NewSphereMenuItem = Template.FindName("ContextMenu_NewSphere", this) as MenuItem;
				NewSphereMenuItem.Click += CreateSubMenuItem_OnClick;
				Hierarchy.HierarchyUpdateEvent += _UpdateHierarchyList;

				hierarchyListBox = Template.FindName("HierarchyList", this) as ListBox;
			}
		}

		private void CreateSubMenuItem_OnClick(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;

			if (item != null)
			{
				switch (item.Header)
				{
					case "Sphere":
						Hierarchy.SceneObject obj = new Hierarchy.SceneObject("Sphere");
						obj.MeshRenderer = new MeshRenderer();
						obj.MeshRenderer.Color = new GLColor(1,1,1,1);
						obj.MeshRenderer.Mesh = new Icosphere(1);
						Hierarchy.HierarchyObjectList.Add(obj);

						Hierarchy.TriggerHierarchyUpdate();
						break;
				}
			}
		}

		private void _UpdateHierarchyList()
		{
			hierarchyListBox.Items.Clear();
			foreach (var sceneObject in Hierarchy.HierarchyObjectList)
			{
				ListBoxItem item = new ListBoxItem();
				item.Content = sceneObject.Name;
				item.Tag = sceneObject.Guid;
				item.ContextMenu = new ContextMenu();
				item.GotFocus += Item_Selected;
				MenuItem deleteMenuItem = new MenuItem();
				deleteMenuItem.Header = "Delete Object";
				deleteMenuItem.Click += DeleteMenuItem_Click;
				deleteMenuItem.Tag = sceneObject.Guid;
				MenuItem focusMenuItem = new MenuItem();
				focusMenuItem.Header = "Focus Object in Scene";
				focusMenuItem.Click += FocusMenuItem_Click;
				focusMenuItem.Tag = sceneObject.Guid;
				item.ContextMenu.Items.Add(deleteMenuItem);
				item.ContextMenu.Items.Add(focusMenuItem);

				hierarchyListBox.Items.Add(item);

			}
		}

		private void FocusMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Control_SceneWindow.OnSceneObjectFocus(Hierarchy.GetObject(item.Tag as string));
		}

		private void Item_Selected(object sender, RoutedEventArgs e)
		{
			ListBoxItem item = sender as ListBoxItem;
			Hierarchy.SceneObject obj = Hierarchy.GetObject(item.Tag as string);
			Hierarchy.ChangeObjectFocus(obj);
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Hierarchy.RemoveObject(item.Tag as string);
			Hierarchy.TriggerHierarchyUpdate();
		}
	}
}
