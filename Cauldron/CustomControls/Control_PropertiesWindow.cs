using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
	[TemplatePart(Name = "PropertyList", Type = typeof(StackPanel))]
	public class Control_PropertiesWindow : Control
	{

		static Control_PropertiesWindow()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_PropertiesWindow), new FrameworkPropertyMetadata(typeof(Control_PropertiesWindow)));
		}

		private StackPanel propertyList;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			if (Template != null)
			{
				propertyList = Template.FindName("PropertyList", this) as StackPanel;
                Hierarchy.FocusChangedEvent += Hierarchy_FocusChangedEvent;
			}
		}

        private void Hierarchy_FocusChangedEvent(Hierarchy.SceneObject obj)
        {
            propertyList.Children.Clear();

            if (obj.properties.ContainsKey("name"))
            {
                propertyList.Children.Add(new Control_Property_Object());
            }

            if (obj.properties.ContainsKey("transform"))
            {
                propertyList.Children.Add(new Control_Property_Transform());
            }

            if (obj.properties.ContainsKey("meshRenderer"))
            {
                propertyList.Children.Add(new Control_Property_MeshRenderer());
            }

            foreach (var propertyListChild in propertyList.Children)
            {
                IProperty iProperty = propertyListChild as IProperty;
                iProperty.UpdateProperty(obj);
            }
        }
	}
}
