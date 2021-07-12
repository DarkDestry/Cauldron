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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Core;
using Transform = System.Windows.Media.Transform;

namespace Cauldron.UserControls.PropertyComponent
{
    /// <summary>
    /// Interaction logic for ObjectInfoControl.xaml
    /// </summary>
    public partial class ObjectInfoControl : UserControl
    {

        public static readonly DependencyProperty ObjectInfoProperty = DependencyProperty.Register(
            "ObjectInfo",
            typeof(Hierarchy.SceneObject.ObjectInfo),
            typeof(ObjectInfoControl));

        public Hierarchy.SceneObject.ObjectInfo ObjectInfo
        {
            get => (Hierarchy.SceneObject.ObjectInfo)GetValue(ObjectInfoProperty);
            set => SetValue(ObjectInfoProperty, value);
        }

        public ObjectInfoControl()
        {
            InitializeComponent();
        }
    }
}
