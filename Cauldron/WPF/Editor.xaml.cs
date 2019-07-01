using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using Color = Microsoft.Xna.Framework.Color;

namespace Cauldron
{
	/// <summary>
	/// Interaction logic for Editor.xaml
	/// </summary>
	public partial class Editor : Window
    {
        [DllImport("elixir_d.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool createBox(float px, float py, float pz, float sx, float sy, float sz, float rx, float ry,
            float rz);
        [DllImport("elixir_d.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern bool render();

        public Editor()
		{
			InitializeComponent();
            this.Loaded += Editor_Loaded;
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            //var obj1 = new Hierarchy.SceneObject("Debug Sphere 1 Axis X");
            //obj1.Transform.Position = new CldVector3(5, 0, 0);
            //obj1.Transform.Scale = new CldVector3(10, 0.5f, 0.5f);
            //var obj2 = new Hierarchy.SceneObject("Debug Sphere 2 Axis Z");
            //obj2.Transform.Position = new CldVector3(0, 0, 5);
            //obj2.Transform.Scale = new CldVector3(0.5f, 0.5f, 10);
            //var obj3 = new Hierarchy.SceneObject("Debug Sphere 3 Axis Y");
            //obj3.Transform.Position = new CldVector3(0, 5, 0);
            //obj3.Transform.Scale = new CldVector3(0.5f, 10, 0.5f);
            var obj4 = new Hierarchy.SceneObject("Debug Sphere 4");
            obj4.Transform.Position = new CldVector3(0, 0.5f, 0);
            obj4.Transform.Scale = new CldVector3(1, 1, 1);
            obj4.Color = Color.SkyBlue;
            var obj5 = new Hierarchy.SceneObject("Debug Sphere 5");
            obj5.Transform.Position = new CldVector3(0, 1.4f, 0);
            obj5.Transform.Scale = new CldVector3(0.8f, 0.8f, 0.8f);
            obj5.Color = Color.Aquamarine;
            var obj6 = new Hierarchy.SceneObject("Debug Sphere 6");
            obj6.Transform.Position = new CldVector3(-0.2f, 1.6f, -0.4f);
            obj6.Transform.Scale = new CldVector3(0.15f, 0.15f, 0.07f);
            obj6.Color = Color.Black;
            var obj7 = new Hierarchy.SceneObject("Debug Sphere 7");
            obj7.Transform.Position = new CldVector3(0.2f, 1.6f, -0.4f);
            obj7.Transform.Scale = new CldVector3(0.15f, 0.15f, 0.07f);
            obj7.Color = Color.Black;
            var obj8 = new Hierarchy.SceneObject("Debug Sphere 8");
            obj8.Transform.Position = new CldVector3(0, 1.2f, -0.4f);
            obj8.Transform.Scale = new CldVector3(0.3f, 0.07f, 0.07f);
            obj8.Color = Color.Pink;



            //Hierarchy.hierarchyObjectList.Add(obj1);
            //Hierarchy.hierarchyObjectList.Add(obj2);
            //Hierarchy.hierarchyObjectList.Add(obj3);
            Hierarchy.hierarchyObjectList.Add(obj4);
            Hierarchy.hierarchyObjectList.Add(obj5);
            Hierarchy.hierarchyObjectList.Add(obj6);
            Hierarchy.hierarchyObjectList.Add(obj7);
            Hierarchy.hierarchyObjectList.Add(obj8);
            Hierarchy.TriggerHierarchyUpdate();
#endif
        }

        private void Render_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (var o in Hierarchy.hierarchyObjectList)
            {

                createBox(o.Transform.Position.x,
                    o.Transform.Position.y,
                    o.Transform.Position.z,
                    o.Transform.Scale.x,
                    o.Transform.Scale.y,
                    o.Transform.Scale.z,
                    o.Transform.Rotation.x,
                    o.Transform.Rotation.y,
                    o.Transform.Rotation.z);
            }

            render();
        }
    }
}

