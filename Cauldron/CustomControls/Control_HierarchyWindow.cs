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

namespace Cauldron.CustomControls
{
	[TemplatePart(Name = "ContextMenu_NewSphere", Type = typeof(MenuItem))]
	[TemplatePart(Name = "HierarchyList", Type = typeof(ListBox))]
	public class Control_HierarchyWindow : Control
	{
		private static Control_HierarchyWindow instance;

		static Control_HierarchyWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_HierarchyWindow), new FrameworkPropertyMetadata(typeof(Control_HierarchyWindow)));
		}

		private ListBox hierarchyListBox;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.Template != null)
			{
				MenuItem NewSphereMenuItem = this.Template.FindName("ContextMenu_NewSphere", this) as MenuItem;
				NewSphereMenuItem.Click += CreateSubMenuItem_OnClick;
				instance = this;

				hierarchyListBox = this.Template.FindName("HierarchyList", this) as ListBox;
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
						Hierarchy.hierarchyObjectList.Add(new Hierarchy.SceneObject("Sphere"));
						_UpdateHierarchyList();
						break;
				}
			}
		}

		public static void UpdateHierarchyList()
		{
			instance._UpdateHierarchyList();
		}

		private void _UpdateHierarchyList()
		{
			hierarchyListBox.Items.Clear();
			foreach (var sceneObject in Hierarchy.hierarchyObjectList)
			{
				ListBoxItem item = new ListBoxItem();
				item.Content = sceneObject.Name;
				item.Tag = sceneObject;
				item.ContextMenu = new ContextMenu();
				item.GotFocus += Item_Selected;
				MenuItem deleteMenuItem = new MenuItem();
				deleteMenuItem.Header = "Delete Object";
				deleteMenuItem.Click += DeleteMenuItem_Click;
				deleteMenuItem.Tag = sceneObject.Guid;
				item.ContextMenu.Items.Add(deleteMenuItem);

				hierarchyListBox.Items.Add(item);

			}
		}

		private void Item_Selected(object sender, RoutedEventArgs e)
		{
			ListBoxItem item = sender as ListBoxItem;
			Hierarchy.SceneObject obj = item.Tag as Hierarchy.SceneObject;
			Control_PropertiesWindow.UpdateProperties(obj.Guid);
		}

		private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
		{
			MenuItem item = sender as MenuItem;
			Hierarchy.RemoveObject(item.Tag as string);
			UpdateHierarchyList();
		}
	}
}
