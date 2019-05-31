using System;
using System.Collections;
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

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "DropDown", Type = typeof(ComboBox))]
    public class Control_Property_DropDown : Control
    {
        #region DependencyProperty
        public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
            "NameLabel",
            typeof(string),
            typeof(Control_Property_DropDown));
        public static readonly DependencyProperty DropDownComboBoxProperty = DependencyProperty.Register(
            "DropDown",
            typeof(IEnumerable),
            typeof(Control_Property_DropDown),
            new FrameworkPropertyMetadata(OnFieldPropertyChanged));

        private static void OnFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control_Property_DropDown cps = d as Control_Property_DropDown;
            if (cps.dropDown != null) cps.dropDown.ItemsSource = e.NewValue as IEnumerable;
        }
        #endregion

        #region CLR Wrapper
        public IEnumerable DropDownComboBox
        {
            get => GetValue(DropDownComboBoxProperty) as IEnumerable;
            set => SetValue(DropDownComboBoxProperty, value);
        }
        public string NameLabel
        {
            get => GetValue(NameLabelProperty) as string;
            set => SetValue(NameLabelProperty, value);
        }
        #endregion

        static Control_Property_DropDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_DropDown), new FrameworkPropertyMetadata(typeof(Control_Property_DropDown)));
        }

        public delegate void FieldChangedEventHandler(string value);

        public event FieldChangedEventHandler FieldChangedEvent;

        private ComboBox dropDown;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                dropDown = Template.FindName("DropDown", this) as ComboBox;
                dropDown.DropDownClosed += TextField_TextChanged;
                dropDown.ItemsSource = DropDownComboBox;
            }
        }

        private void TextField_TextChanged(object sender, EventArgs eventArgs)
        {
            //TextField = DropDown.Text;

            OnFieldChanged(dropDown.Text);
        }

        public void UpdateProperty(object value)
        {
            string s = value as string;
            //TextField = s;
        }

        protected virtual void OnFieldChanged(string value) => FieldChangedEvent?.Invoke(value);
    }
}
