using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography.X509Certificates;
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
	[TemplatePart(Name = "Field_Text", Type = typeof(TextBox))]
	public class Control_Property_String : Control, IProperty
	{

		public static readonly DependencyProperty NameLabelProperty = DependencyProperty.Register(
			"NameLabel",
			typeof(string),
			typeof(Control_Property_String));
		public static readonly DependencyProperty TextFieldProperty = DependencyProperty.Register(
			"Field_Text",
			typeof(string),
			typeof(Control_Property_String),
			new FrameworkPropertyMetadata(OnFieldPropertyChanged));

		private static void OnFieldPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			Control_Property_String cps = d as Control_Property_String;
			cps.textField.Text = e.NewValue as string;
		}

		public string TextField
		{
			get => (string) GetValue(TextFieldProperty);
			set => SetValue(TextFieldProperty, value);
		}
		public string NameLabel
		{
			get => (string) GetValue(NameLabelProperty);
			set => SetValue(NameLabelProperty, value);
		}

		static Control_Property_String()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_String), new FrameworkPropertyMetadata(typeof(Control_Property_String)));
		}

		public delegate void FieldChangedEventHandler(string value);

		public event FieldChangedEventHandler FieldChangedEvent;

		private TextBox textField;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (this.Template != null)
			{
				textField = this.Template.FindName("Field_Text", this) as TextBox;
				textField.TextChanged += TextField_TextChanged;
			}
		}

		private void TextField_TextChanged(object sender, TextChangedEventArgs e)
		{
			OnFieldChanged(textField.Text);
		}

		public void UpdateProperty(object value)
		{
			string s = value as string;
			TextField = s;
		}

		protected virtual void OnFieldChanged(string value)
		{
			FieldChangedEvent?.Invoke(value);
		}
	}
}
