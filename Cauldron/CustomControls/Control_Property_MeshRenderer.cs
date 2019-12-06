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
using SharpGL.SceneGraph;

namespace Cauldron.CustomControls
{

    [TemplatePart(Name = "Vector3_Color", Type = typeof(Control_Property_Vector3))]
    public class Control_Property_MeshRenderer : Control , IProperty
    {

        private string currentObjectGuid = "";

        static Control_Property_MeshRenderer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_Property_MeshRenderer), new FrameworkPropertyMetadata(typeof(Control_Property_MeshRenderer)));
        }

        private Control_Property_Vector3 color;
        private bool templateApplied;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            templateApplied = true;

            if (Template != null)
            {
                color = Template.FindName("Vector3_Color", this) as Control_Property_Vector3;
                color.FieldChangedEvent += Color_FieldChanged;
                if (currentObjectGuid != null) UpdateProperty(Hierarchy.GetObject(currentObjectGuid));
            }
        }

        private void Color_FieldChanged(CldVector3 value)
        {
            Hierarchy.GetObject(currentObjectGuid).MeshRenderer.Color = (GLColor) value;
        }

        public void UpdateProperty(object value)
        {
            Hierarchy.SceneObject obj = value as Hierarchy.SceneObject;

            currentObjectGuid = obj.Guid;

            if (!templateApplied) return;

            color.UpdateProperty(obj.MeshRenderer.Color);
        }
    }
}
