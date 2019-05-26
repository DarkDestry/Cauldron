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

        #endregion


        public delegate void FieldChangedEventHandler(Vector3 value);

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
            }
        }


        static Control_Property_Vector3()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_Vector3), new FrameworkPropertyMetadata(typeof(Control_Property_Vector3)));

        }

        private TextBox x, y, z;
        private bool templateApplied;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            templateApplied = true;

            if (Template != null)
            {
                x = Template.FindName("Field_X",this) as TextBox;
                y = Template.FindName("Field_Y",this) as TextBox;
                z = Template.FindName("Field_Z",this) as TextBox;
                x.PreviewTextInput += PreviewTextInput;
                y.PreviewTextInput += PreviewTextInput;
                z.PreviewTextInput += PreviewTextInput;

                x.TextChanged += Vector3_TextChanged;
                y.TextChanged += Vector3_TextChanged;
                z.TextChanged += Vector3_TextChanged;
                x.Text = XValue.ToString();
                y.Text = YValue.ToString();
                z.Text = ZValue.ToString();
            }
        }

        #region Input Validation
        private void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox field = sender as TextBox;

            e.Handled = !IsTextAllowed(field.Text + e.Text);
        }


        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        private static bool IsTextAllowed(string text)
        {
            
            float f;
            return float.TryParse(text, out f);
            //return !_regex.IsMatch(text);
        }

        #endregion

        private void Vector3_TextChanged(object sender, TextChangedEventArgs e)
        {

            float xV = x.Text == "" ? 0 : float.Parse(x.Text);
            float yV = y.Text == "" ? 0 : float.Parse(y.Text);
            float zV = z.Text == "" ? 0 : float.Parse(z.Text);

            TextBox field = sender as TextBox;
            switch (field.Name)
            {
                case "Field_X":
                    XValue = xV;
                    break;
                case "Field_Y":
                    YValue = yV;
                    break;
                case "Field_Z":
                    ZValue = zV;
                    break;
            }

            Vector3 v3 = new Vector3(xV,yV,zV);
            OnFieldChanged(v3);
        }

        public void UpdateProperty(object value)
        {
            Vector3 v3 = value is Vector3 ? (Vector3) value : default;

            XValue = v3.x;
            YValue = v3.y;
            ZValue = v3.z;
        }

        protected virtual void OnFieldChanged(Vector3 value) => FieldChangedEvent?.Invoke(value);
    }
}
