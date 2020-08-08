using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
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


    [TemplatePart(Name = "Field_X", Type = typeof(TextBox))]
    [TemplatePart(Name = "Field_Y", Type = typeof(TextBox))]
    [TemplatePart(Name = "Field_Z", Type = typeof(TextBox))]
    public class Control_Property_Vector3 : Control, IProperty
    {
        #region DependencyProperties

        public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
            "NameLabel",
            typeof(string),
            typeof(Control_Property_Vector3));

        public static readonly DependencyProperty XValueProperty = DependencyProperty.Register(
            "XValue",
            typeof(float),
            typeof(Control_Property_Vector3),
            new FrameworkPropertyMetadata(OnFloatPropertyChanged));

        public static readonly DependencyProperty YValueProperty = DependencyProperty.Register(
            "YValue",
            typeof(float),
            typeof(Control_Property_Vector3),
            new FrameworkPropertyMetadata(OnFloatPropertyChanged));

        public static readonly DependencyProperty ZValueProperty = DependencyProperty.Register(
            "ZValue",
            typeof(float),
            typeof(Control_Property_Vector3),
            new FrameworkPropertyMetadata(OnFloatPropertyChanged));

        public static readonly DependencyProperty SensitivityProperty = DependencyProperty.Register(
            "Sensitivity", 
            typeof(float), 
            typeof(Control_Property_Vector3),
            new FrameworkPropertyMetadata(OnFloatPropertyChanged));

        #endregion

        #region CLR Wrappers
        public string NameLabel
        {
            get => (string)GetValue(NameLabelProperty);
            set => SetValue(NameLabelProperty, value);
        }

        public float XValue
        {
            get => (float)GetValue(XValueProperty);
            set => SetValue(XValueProperty, value);
        }

        public float YValue
        {
            get => (float)GetValue(YValueProperty);
            set => SetValue(YValueProperty, value);
        }

        public float ZValue
        {
            get => (float)GetValue(ZValueProperty);
            set => SetValue(ZValueProperty, value);
        }

        public float Sensitivity
        {
            get => (float)GetValue(SensitivityProperty);
            set => SetValue(SensitivityProperty, value);
        }

        #endregion


        public delegate void FieldChangedEventHandler(CldVector3 value);

        public event FieldChangedEventHandler FieldChangedEvent;

        private static void OnFloatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control_Property_Vector3 cpv3 = d as Control_Property_Vector3;
            if (!cpv3.templateApplied) return;
            switch (e.Property.Name)
            {
                case "XValue":
                {
                    int caretIndex = cpv3.x.CaretIndex;
                    cpv3.x.Text = e.NewValue.ToString();
                    cpv3.x.CaretIndex = caretIndex;
                }
                    break;
                case "YValue":
                {
                    int caretIndex = cpv3.y.CaretIndex;
                    cpv3.y.Text = e.NewValue.ToString();
                    cpv3.y.CaretIndex = caretIndex;
                }
                    break;
                case "ZValue":
                {
                    int caretIndex = cpv3.z.CaretIndex;
                    cpv3.z.Text = e.NewValue.ToString();
                    cpv3.z.CaretIndex = caretIndex;
                }
                    break;
                case "Sensitivity":
                {
                    cpv3.sensitivity = float.Parse(e.NewValue.ToString());
                }
                break;
            }
        }


        static Control_Property_Vector3()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_Vector3), new FrameworkPropertyMetadata(typeof(Control_Property_Vector3)));

        }

        private TextBox x, y, z;
        private bool templateApplied;
        private float sensitivity = 0.25f;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            templateApplied = true;

            if (Template != null)
            {
                x = Template.FindName("Field_X",this) as TextBox;
                y = Template.FindName("Field_Y",this) as TextBox;
                z = Template.FindName("Field_Z",this) as TextBox;

                x.TextChanged += Vector3_TextChanged;
                y.TextChanged += Vector3_TextChanged;
                z.TextChanged += Vector3_TextChanged;
                x.Text = XValue.ToString();
                y.Text = YValue.ToString();
                z.Text = ZValue.ToString();

                x.PreviewMouseDown += MouseMiddleButtonDown;
                y.PreviewMouseDown += MouseMiddleButtonDown;
                z.PreviewMouseDown += MouseMiddleButtonDown;

                x.MouseMove += Field_MouseMove;
                y.MouseMove += Field_MouseMove;
                z.MouseMove += Field_MouseMove;
                x.GotMouseCapture += Field_MouseMove;
                y.GotMouseCapture += Field_MouseMove;
                z.GotMouseCapture += Field_MouseMove;

                sensitivity = Sensitivity;
            }
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
            }

            if (dragging)
            {
                Vector delta = e.GetPosition(field) - cursorPos;

                cursorPos = e.GetPosition(field);

                bool isShiftHeld = Keyboard.GetKeyStates(Key.LeftShift) == KeyStates.Down;
                bool isCtrlHeld = Keyboard.GetKeyStates(Key.LeftCtrl) == KeyStates.Down;

                float multiplier = isShiftHeld ? 3 : isCtrlHeld ? 0.3f : 1;

                switch (field.Name)
                {
                    case "Field_X":
                        XValue = (float)(float.Parse(field.Text) + delta.X * sensitivity * multiplier);
                        break;
                    case "Field_Y":
                        YValue = (float)(float.Parse(field.Text) + delta.X * sensitivity * multiplier);
                        break;
                    case "Field_Z":
                        ZValue = (float)(float.Parse(field.Text) + delta.X * sensitivity * multiplier);
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

        private void MouseMiddleButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                Mouse.Capture(sender as IInputElement);
                Mouse.OverrideCursor = Cursors.SizeWE;
            }
        }

        private void Vector3_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox field = sender as TextBox;
            UpdateField(field.Name, field.Text);

            CldVector3 v3 = new CldVector3(XValue,YValue,ZValue);
            OnFieldChanged(v3);
        }

        private void UpdateField(string fieldName, string value)
        {
            float V;
            if (!float.TryParse(value, out V)) return;

            switch (fieldName)
            {
                case "Field_X":
                    XValue = V;
                    break;
                case "Field_Y":
                    YValue = V;
                    break;
                case "Field_Z":
                    ZValue = V;
                    break;
            }
        }

        public void UpdateProperty(object value)
        {
            CldVector3 v3 = value is CldVector3 ? (CldVector3) value : default;

            XValue = v3.x;
            YValue = v3.y;
            ZValue = v3.z;
        }

        protected virtual void OnFieldChanged(CldVector3 value) => FieldChangedEvent?.Invoke(value);
    }
}
