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
	[TemplatePart(Name = "String_Name", Type = typeof(Control_Property_String))]
	[TemplatePart(Name = "String_Guid", Type = typeof(Control_Property_String))]
	public class Control_Property_Object : Control, IProperty
	{
		private string currentObjectGuid = "";

		static Control_Property_Object()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_Object), new FrameworkPropertyMetadata(typeof(Control_Property_Object)));
		}

		private Control_Property_String name, guid;
        private bool templateApplied;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            templateApplied = true;

            if (Template != null)
			{
				name = Template.FindName("String_Name", this) as Control_Property_String;
				name.FieldChangedEvent += Name_FieldChanged;
				guid = Template.FindName("String_Guid", this) as Control_Property_String;
                if (currentObjectGuid != null) UpdateProperty(Hierarchy.GetObject(currentObjectGuid));
			}
        }

		private void Name_FieldChanged(string value)
		{
			Hierarchy.GetObject(currentObjectGuid).Name = value;
			Control_HierarchyWindow.UpdateHierarchyList();
		}

		public void UpdateProperty(object value)
        {
			Hierarchy.SceneObject obj = value as Hierarchy.SceneObject;

			currentObjectGuid = obj.Guid;

            if (!templateApplied) return;

            name.UpdateProperty(obj.Name);
			guid.UpdateProperty(obj.Guid);
		}
	}
}
