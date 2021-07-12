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

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "Label_Value", Type = typeof(Label))]
    public class Control_Property_ReadOnly : Control
    {
        #region DependencyProperty
        public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
            "NameLabel",
            typeof(string),
            typeof(Control_Property_ReadOnly));
        public static readonly DependencyProperty LabelValueProperty = DependencyProperty.Register(
            "Label_Value",
            typeof(string),
            typeof(Control_Property_ReadOnly),
            new FrameworkPropertyMetadata(OnFieldPropertyChanged));

        private static void OnFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Control_Property_ReadOnly cps = d as Control_Property_ReadOnly;
            if (cps.label != null) cps.label.Content = e.NewValue as string;
        }
        #endregion

        #region CLR Wrapper
        public string LabelValue
        {
            get => (string)GetValue(LabelValueProperty);
            set => SetValue(LabelValueProperty, value);
        }
        public string NameLabel
        {
            get => (string)GetValue(NameLabelProperty);
            set => SetValue(NameLabelProperty, value);
        }
        #endregion 

        static Control_Property_ReadOnly()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_ReadOnly), new FrameworkPropertyMetadata(typeof(Control_Property_ReadOnly)));
        }

        private Label label;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (Template != null)
            {
                label = Template.FindName("Label_Value", this) as Label;
                label.Content = LabelValue;
            }
        }

        public void UpdateProperty(object value)
        {
            string s = value as string;
            LabelValue = s;
        }
    }
}
