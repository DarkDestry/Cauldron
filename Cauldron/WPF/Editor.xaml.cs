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
using Cauldron.Core;
using Cauldron.CustomControls;
using Cauldron.Primitives;
using Color = System.Drawing.Color;

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

#if DEBUG
            var body = new Hierarchy.SceneObject("Debug Cube 4");
            body.Transform.Position = new CldVector3(0, 0.5f, 0);
            body.Transform.Scale = new CldVector3(1, 1, 1);
            body.MeshRenderer = new MeshRenderer { Color = Color.SkyBlue, Mesh = new Box() };
            var head = new Hierarchy.SceneObject("Debug Cube 5");
            head.Transform.Position = new CldVector3(0, 1.4f, 0);
            head.Transform.Scale = new CldVector3(0.8f, 0.8f, 0.8f);
            head.MeshRenderer = new MeshRenderer { Color = Color.Aquamarine, Mesh = new Box() };
            var eye1 = new Hierarchy.SceneObject("Debug Sphere 6");
            eye1.Transform.Position = new CldVector3(-0.2f, 1.6f, -0.4f);
            eye1.Transform.Scale = new CldVector3(0.15f, 0.15f, 0.07f);
            eye1.MeshRenderer = new MeshRenderer { Color = Color.Black, Mesh = new Icosphere(1) };
            var eye2 = new Hierarchy.SceneObject("Debug Sphere 7");
            eye2.Transform.Position = new CldVector3(0.2f, 1.6f, -0.4f);
            eye2.Transform.Scale = new CldVector3(0.15f, 0.15f, 0.07f);
            eye2.MeshRenderer = new MeshRenderer { Color = Color.Black, Mesh = new Icosphere(1) };
            var mouth = new Hierarchy.SceneObject("Debug Sphere 8");
            mouth.Transform.Position = new CldVector3(0, 1.2f, -0.4f);
            mouth.Transform.Scale = new CldVector3(0.3f, 0.07f, 0.07f);
            mouth.MeshRenderer = new MeshRenderer { Color = Color.Pink, Mesh = new Icosphere(1) };


            Hierarchy.HierarchyObjectList.Add(body);
            Hierarchy.HierarchyObjectList.Add(head);
            Hierarchy.HierarchyObjectList.Add(eye1);
            Hierarchy.HierarchyObjectList.Add(eye2);
            Hierarchy.HierarchyObjectList.Add(mouth);
#endif
        }

        private void Editor_Loaded(object sender, RoutedEventArgs e)
        {
#if DEBUG
            Hierarchy.TriggerHierarchyUpdate();
#endif
        }
    }
}

