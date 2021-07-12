using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Cauldron.CustomControls;
using Cauldron.Primitives;
using SharpGL.SceneGraph;

namespace Cauldron.UserControls.Windows
{
    /// <summary>
    /// Interaction logic for HierarchyWindow.xaml
    /// </summary>
    public partial class HierarchyWindow : UserControl
    {

        public HierarchyWindow()
        {
            InitializeComponent();

            ContextMenu_NewSphere.Click += CreateSubMenuItem_OnClick;
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

        private void FocusMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            // Control_SceneWindow.OnSceneObjectFocus(Hierarchy.GetObject(item.Tag as string));
            //TODO: Fix this to handle multiple viewports
            SceneWindow.Renderer.SetCameraFocus(Hierarchy.GetObject(item.Tag as string).Transform.Position);
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
