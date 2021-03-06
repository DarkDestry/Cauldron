﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cauldron.Core;
using Control = System.Windows.Controls.Control;
using MessageBox = System.Windows.MessageBox;

namespace Cauldron.CustomControls
{


	[TemplatePart(Name = "Vector3_Position", Type = typeof(Control_Property_Vector3))]
	[TemplatePart(Name = "Vector3_Rotation", Type = typeof(Control_Property_Vector3))]
	[TemplatePart(Name = "Vector3_Scale", Type = typeof(Control_Property_Vector3))]
	public class Control_Property_Transform : Control, IProperty
	{
		private string currentObjectGuid = "";

		static Control_Property_Transform()
		{
			DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_Transform), new FrameworkPropertyMetadata(typeof(Control_Property_Transform)));
		}

		private Control_Property_Vector3 position, rotation, scale;
        private bool templateApplied;

		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

            templateApplied = true;

            if (Template != null)
			{
				position = Template.FindName("Vector3_Position", this) as Control_Property_Vector3;
				position.FieldChangedEvent += Position_FieldChanged;
				rotation = Template.FindName("Vector3_Rotation", this) as Control_Property_Vector3;
				rotation.FieldChangedEvent += Rotation_FieldChanged;
				scale = Template.FindName("Vector3_Scale", this) as Control_Property_Vector3;
				scale.FieldChangedEvent += Scale_FieldChanged;
                if (!string.IsNullOrEmpty(currentObjectGuid)) UpdateProperty(Hierarchy.GetObject(currentObjectGuid));
            }
        }

		private void Scale_FieldChanged(CldVector3 value)
        {
            Hierarchy.GetObject(currentObjectGuid).Transform.Scale = value;
            Hierarchy.TriggerHierarchyUpdate();
        }

        private void Rotation_FieldChanged(CldVector3 value)
        {
            Hierarchy.GetObject(currentObjectGuid).Transform.Rotation = value;
            Hierarchy.TriggerHierarchyUpdate();
        }

        private void Position_FieldChanged(CldVector3 value)
        {
            Hierarchy.GetObject(currentObjectGuid).Transform.Position = value;
            Hierarchy.TriggerHierarchyUpdate();
        }

        public void UpdateProperty(object value)
		{
			Hierarchy.SceneObject sceneObject = value as Hierarchy.SceneObject;

			currentObjectGuid = sceneObject.Guid;

            if (!templateApplied) return;

			position.UpdateProperty(sceneObject.Transform.Position);
			rotation.UpdateProperty(sceneObject.Transform.Rotation);			
			scale.UpdateProperty(sceneObject.Transform.Scale);
        }
	}
}
