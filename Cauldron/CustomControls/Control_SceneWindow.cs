using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "Canvas_Front", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Right", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Top", Type = typeof(Canvas))]
    [TemplatePart(Name = "Viewport_3D", Type = typeof(MonoGameViewport))]
    public class Control_SceneWindow : Control
    {
        private delegate void SceneObjectFocusEventHandler(Hierarchy.SceneObject obj);

        private static event SceneObjectFocusEventHandler SceneObjectFocusEvent;

        public static void OnSceneObjectFocus(Hierarchy.SceneObject
            obj) => SceneObjectFocusEvent?.Invoke(obj);


        static Control_SceneWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_SceneWindow), new FrameworkPropertyMetadata(typeof(Control_SceneWindow)));
        }


        //private Canvas front, right, top;
        private MonoGameViewport perspective;
        private int frontZoom = 8;
        private int rightZoom = 8;
        private int topZoom = 8;

        //Centralized Focus Point
        private CldVector3 focusPoint = new CldVector3(0,0,0);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
                 
            if (Template != null)
            {
            }
        }

        private void FocusObject(Hierarchy.SceneObject obj)
        {
            focusPoint = obj.Transform.Position;
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as IInputElement);
        }

        private bool panning;
        private Point pos;

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            if (e.MiddleButton == MouseButtonState.Pressed && !panning)
            {
                pos = Mouse.GetPosition(canvas);
                panning = true;
            }

            if (panning)
            {
                Vector delta = e.GetPosition(canvas) - pos;

                pos = e.GetPosition(canvas);

                switch (canvas.Name)
                {
                    case "Canvas_Front":
                        focusPoint.x += -(float)delta.X * 1 / frontZoom;
                        focusPoint.y += (float)delta.Y * 1 / frontZoom;
                        break;
                    case "Canvas_Right":
                        focusPoint.z += (float)delta.X * 1 / rightZoom;
                        focusPoint.y += (float)delta.Y * 1 / rightZoom;
                        break;
                    case "Canvas_Top":
                        focusPoint.x += -(float)delta.X * 1 / topZoom;
                        focusPoint.z += -(float)delta.Y * 1 / topZoom;
                        break;
                    case "Canvas_3D":
                        //TODO: 3D canvas movement
                        break;
                }
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    panning = false;
                    Mouse.Capture(null);
                }
            }
        }
    }
}
