using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Cauldron.Annotations;
using Cauldron.Core;

namespace Cauldron.UserControls.PropertyComponent
{
    /// <summary>
    /// Interaction logic for TransformControl.xaml
    /// </summary>
    public partial class TransformControl : UserControl, INotifyPropertyChanged
    {
        // public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
        //     "Position",
        //     typeof(CldVector3),
        //     typeof(TransformControl));
        //
        // public static readonly DependencyProperty RotationProperty = DependencyProperty.Register(
        //     "Rotation",
        //     typeof(CldVector3),
        //     typeof(TransformControl));
        //
        // public static readonly DependencyProperty ScaleProperty = DependencyProperty.Register(
        //     "Scale",
        //     typeof(CldVector3),
        //     typeof(TransformControl));
        //
        //
        // public CldVector3 Position
        // {
        //     get => (CldVector3)GetValue(PositionProperty);
        //     set => SetValue(PositionProperty, value);
        // }
        // public CldVector3 Rotation
        // {
        //     get => (CldVector3)GetValue(RotationProperty);
        //     set => SetValue(RotationProperty, value);
        // }
        // public CldVector3 Scale
        // {
        //     get => (CldVector3)GetValue(ScaleProperty);
        //     set => SetValue(ScaleProperty, value);
        // }

        // private CldVector3 _position;
        // private CldVector3 _rotation;
        // private CldVector3 _scale;
        //
        // public CldVector3 Position
        // {
        //     get => _position;
        //     set
        //     {
        //         _position = value;
        //         OnPropertyChanged();
        //     }
        // }
        //
        // public CldVector3 Rotation
        // {
        //     get => _rotation;
        //     set 
        //     {
        //         _rotation = value;
        //         OnPropertyChanged();
        //     }
        // }
        //
        // public CldVector3 Scale
        // {
        //     get => _scale;
        //     set 
        //     {
        //         _scale = value;
        //         OnPropertyChanged();
        //     }
        // }

        public static readonly DependencyProperty TransformProperty = DependencyProperty.Register(
            "Transform",
            typeof(Transform),
            typeof(TransformControl));

        public Transform Transform
        {
            get => (Transform)GetValue(TransformProperty);
            set => SetValue(TransformProperty, value);
        }

        public TransformControl()
        {
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
