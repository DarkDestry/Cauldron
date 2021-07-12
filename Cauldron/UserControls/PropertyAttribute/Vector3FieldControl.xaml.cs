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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Annotations;

namespace Cauldron.UserControls.PropertyAttribute
{
    /// <summary>
    /// Interaction logic for Vector3FieldControl.xaml
    /// </summary>
    public partial class Vector3FieldControl : UserControl, INotifyPropertyChanged
    {
        #region DependencyProperties

        public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
            "NameLabel",
            typeof(string),
            typeof(Vector3FieldControl));

        public static readonly DependencyProperty SensitivityProperty = DependencyProperty.Register(
            "Sensitivity", 
            typeof(float), 
            typeof(Vector3FieldControl));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", 
            typeof(CldVector3), 
            typeof(Vector3FieldControl));

        #endregion

        #region CLR Wrappers
        public string NameLabel
        {
            get => (string)GetValue(NameLabelProperty);
            set
            {
                SetValue(NameLabelProperty, value); 
                OnPropertyChanged();
            }
        }

        public float Sensitivity
        {
            get => (float)GetValue(SensitivityProperty);
            set => SetValue(SensitivityProperty, value);
        }

        public CldVector3 Value
        {
            get => (CldVector3)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        #endregion

        // private CldVector3 _value;
        //
        // public CldVector3 Value
        // {
        //     get => _value;
        //     set
        //     {
        //         _value = value;
        //         OnPropertyChanged();
        //     }
        // }

        public Vector3FieldControl()
        {
            InitializeComponent();
        }

        public override void OnApplyTemplate()
        {
            OnPropertyChanged("NameLabel");
            base.OnApplyTemplate();
        }

        private bool dragging;
        private Point cursorPos;

        private void Field_MouseMove(object sender, MouseEventArgs e)
        {
            TextBox field = sender as TextBox;

            if (!dragging && e.MiddleButton == MouseButtonState.Pressed)
            {
                dragging = true;
                cursorPos = Mouse.GetPosition(field);
                Mouse.Capture(field);
            }

            if (dragging)
            {
                Vector delta = e.GetPosition(field) - cursorPos;

                cursorPos = e.GetPosition(field);

                bool isShiftHeld = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down;
                bool isCtrlHeld = Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down;

                float multiplier = isShiftHeld ? 3 : isCtrlHeld ? 0.3f : 1;
                float valueDelta = (float) (float.Parse(field.Text) + delta.X * Sensitivity * multiplier);

                switch (field.Name)
                {
                    case "Field_X":
                        Value = new CldVector3(valueDelta, Value.y, Value.z);
                        break;
                    case "Field_Y":
                        Value = new CldVector3(Value.x, valueDelta, Value.z);
                        break;
                    case "Field_Z":
                        Value = new CldVector3(Value.x, Value.y, valueDelta);
                        break;
                }


                if (e.MiddleButton == MouseButtonState.Released)
                {
                    dragging = false;
                    Mouse.OverrideCursor = Cursors.Arrow;
                    Mouse.Capture(null);
                }
            }
        }

        private void FieldLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            float x, y, z;

            if (!float.TryParse(Field_X.Text, out x)
                || !float.TryParse(Field_Y.Text, out y)
                || !float.TryParse(Field_Z.Text, out z))
            {
                Field_X.Text = Value.X.ToString();
                Field_Y.Text = Value.Y.ToString();
                Field_Z.Text = Value.Z.ToString();
                MessageBox.Show("Invalid input. Please enter a floating point value.");
                return;
            }

            Value = new CldVector3(x, y, z);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
