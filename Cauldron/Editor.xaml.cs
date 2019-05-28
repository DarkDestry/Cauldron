using System;
using System.Collections.Generic;
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
using Cauldron.CustomControls;

namespace Cauldron
{
	/// <summary>
	/// Interaction logic for Editor.xaml
	/// </summary>
	public partial class Editor : Window
	{
		public Editor()
		{
			InitializeComponent();
            this.Loaded += Editor_Loaded;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            var obj1 = new Hierarchy.SceneObject("Debug Sphere 1");
            obj1.Transform.Position = new Vector3(50,0,0);
            obj1.Transform.Scale = new Vector3(100,5,5);
            var obj2 = new Hierarchy.SceneObject("Debug Sphere 2");
            obj2.Transform.Position = new Vector3(0,0,50);
            obj2.Transform.Scale = new Vector3(5,5,100);
            var obj3 = new Hierarchy.SceneObject("Debug Sphere 3");
            obj3.Transform.Position = new Vector3(0, 50, 0);
            obj3.Transform.Scale = new Vector3(5,100,5);
            var obj4 = new Hierarchy.SceneObject("Debug Sphere 4");
            obj4.Transform.Position = new Vector3(100, 40, 100);
            obj4.Transform.Scale = new Vector3(80, 80, 80);
            var obj5 = new Hierarchy.SceneObject("Debug Sphere 5");
            obj5.Transform.Position = new Vector3(100, 110, 100);
            obj5.Transform.Scale = new Vector3(60, 60, 60);
            var obj6 = new Hierarchy.SceneObject("Debug Sphere 6");
            obj6.Transform.Position = new Vector3(110, 120, 125);
            obj6.Transform.Scale = new Vector3(15, 15, 15);
            var obj7 = new Hierarchy.SceneObject("Debug Sphere 7");
            obj7.Transform.Position = new Vector3(90, 120, 125);
            obj7.Transform.Scale = new Vector3(15, 15, 15);
            var obj8 = new Hierarchy.SceneObject("Debug Sphere 8");
            obj8.Transform.Position = new Vector3(100, 100, 125);
            obj8.Transform.Scale = new Vector3(30, 7, 7);



            Hierarchy.hierarchyObjectList.Add(obj1);
            Hierarchy.hierarchyObjectList.Add(obj2);
            Hierarchy.hierarchyObjectList.Add(obj3);
            Hierarchy.hierarchyObjectList.Add(obj4);
            Hierarchy.hierarchyObjectList.Add(obj5);
            Hierarchy.hierarchyObjectList.Add(obj6);
            Hierarchy.hierarchyObjectList.Add(obj7);
            Hierarchy.hierarchyObjectList.Add(obj8);
            Hierarchy.TriggerHierarchyUpdate();
#endif
        }
    }
}

