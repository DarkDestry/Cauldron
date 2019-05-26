using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [TemplatePart(Name = "Canvas_Front", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Right", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Top", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_3D", Type = typeof(Canvas))]
    public class Control_SceneWindow : Control
    {
        static Control_SceneWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Control_SceneWindow), new FrameworkPropertyMetadata(typeof(Control_SceneWindow)));
        }


        private Canvas front, right, top, perspective;

        //Centralized Focus Point
        private Vector3 focusPoint = new Vector3(0,0,0);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
                 
            if (Template != null)
            {
                front = Template.FindName("Canvas_Front", this) as Canvas;
                right = Template.FindName("Canvas_Right",this) as Canvas;
                top = Template.FindName("Canvas_Top",this) as Canvas;
                perspective = Template.FindName("Canvas_3D",this) as Canvas;

                front.GotMouseCapture += Front_GotMouseCapture;
                right.GotMouseCapture += Right_GotMouseCapture;
                top.GotMouseCapture += Top_GotMouseCapture;
                //perspective.GotMouseCapture += Perspective_GotMouseCapture;

                front.MouseDown += Canvas_MouseDown;
                right.MouseDown += Canvas_MouseDown;
                top.MouseDown += Canvas_MouseDown;
                //perspective.MouseDown += Canvas_MouseDown;

                front.MouseMove += Front_GotMouseCapture;
                right.MouseMove += Right_GotMouseCapture;
                top.MouseMove += Top_GotMouseCapture;
                //perspective.GotMouseCapture += Perspective_GotMouseCapture;
            }

            UpdateCanvas();
        }

        private void Canvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(sender as IInputElement);
        }

        private void UpdateCanvas()
        {
            front.Children.Clear();
            Label test = new Label();
            test.Content = $"{focusPoint.x}, {focusPoint.y}, {focusPoint.z}";
            front.Children.Add(test);
        }

        private bool panning;
        private Point pos;

        private void Perspective_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && !panning)
            {
                pos = Mouse.GetPosition(sender as Canvas);
                panning = true;
            }

            if (panning) {
                Vector delta = e.GetPosition(sender as Canvas) - pos;

                pos = e.GetPosition(sender as Canvas);

                focusPoint.x += (float)delta.X;
                focusPoint.y += (float)delta.Y;
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    panning = false;
                    Mouse.Capture(null);
                }
            }
            UpdateCanvas();
        }

        private void Top_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && !panning)
            {
                pos = Mouse.GetPosition(sender as Canvas);
                panning = true;
            }

            if (panning)
            {
                Vector delta = e.GetPosition(sender as Canvas) - pos;

                pos = e.GetPosition(sender as Canvas);

                focusPoint.x += -(float)delta.X;
                focusPoint.z += -(float)delta.Y;
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    panning = false;
                    Mouse.Capture(null);
                }
            }
            UpdateCanvas();
        }

        private void Right_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && !panning)
            {
                pos = Mouse.GetPosition(sender as Canvas);
                panning = true;
            }

            if (panning)
            {
                Vector delta = e.GetPosition(sender as Canvas) - pos;

                pos = e.GetPosition(sender as Canvas);

                focusPoint.z += (float)delta.X;
                focusPoint.y += (float)delta.Y;
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    panning = false;
                    Mouse.Capture(null);
                }
            }
            UpdateCanvas();
        }

        private void Front_GotMouseCapture(object sender, MouseEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed && !panning)
            {
                pos = Mouse.GetPosition(sender as Canvas);
                panning = true;
            }

            if (panning)
            {
                Vector delta = e.GetPosition(sender as Canvas) - pos;

                pos = e.GetPosition(sender as Canvas);

                focusPoint.x += -(float)delta.X;
                focusPoint.y += (float)delta.Y;
                if (e.MiddleButton == MouseButtonState.Released)
                {
                    panning = false;
                    Mouse.Capture(null);
                }
            }
            UpdateCanvas();
        }
    }
}
