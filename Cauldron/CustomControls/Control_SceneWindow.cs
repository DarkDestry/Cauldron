using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
        private int frontZoom = 1;
        private int rightZoom = 1;
        private int topZoom = 1;

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

                front.GotMouseCapture += HandleMouseMove;
                right.GotMouseCapture += HandleMouseMove;
                top.GotMouseCapture += HandleMouseMove;
                //perspective.GotMouseCapture += Perspective_GotMouseCapture;

                front.MouseDown += Canvas_MouseDown;
                right.MouseDown += Canvas_MouseDown;
                top.MouseDown += Canvas_MouseDown;
                //perspective.MouseDown += Canvas_MouseDown;

                front.MouseMove += HandleMouseMove;
                right.MouseMove += HandleMouseMove;
                top.MouseMove += HandleMouseMove;
                //perspective.GotMouseCapture += Perspective_GotMouseCapture;

                front.MouseWheel += (sender, args) =>
                {
                    if (Math.Sign(args.Delta) > 0) frontZoom *= 2;
                    else frontZoom /= 2;
                    UpdateCanvas();
                };
                right.MouseWheel += (sender, args) =>
                {
                    if (Math.Sign(args.Delta) > 0) rightZoom *= 2;
                    else rightZoom /= 2;
                    UpdateCanvas();
                };
                top.MouseWheel += (sender, args) =>
                {
                    if (Math.Sign(args.Delta) > 0) topZoom *= 2;
                    else topZoom /= 2;
                    UpdateCanvas();
                };

                Hierarchy.HierarchyUpdateEvent += UpdateCanvas;
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
            right.Children.Clear();
            top.Children.Clear();
            Label test = new Label();
            test.Content = $"{focusPoint.x}, {focusPoint.y}, {focusPoint.z}";
            front.Children.Add(test);

            //canvas top left points
            float frontX = focusPoint.x - (float) front.ActualWidth / 2 / frontZoom;
            float frontY = focusPoint.y + (float) front.ActualHeight / 2 / frontZoom;
            float rightX = focusPoint.z + (float) right.ActualWidth / 2 / rightZoom;
            float rightY = focusPoint.y + (float) right.ActualHeight / 2 / rightZoom; 
            float topX = focusPoint.x - (float) top.ActualWidth / 2 / topZoom;
            float topY = focusPoint.z - (float)top.ActualHeight / 2 / topZoom;


            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                //Top Canvas
                Ellipse topSphere = new Ellipse();
                topSphere.Width = sceneObject.Transform.Scale.x * topZoom;
                topSphere.Height = sceneObject.Transform.Scale.z * topZoom;
                topSphere.Stroke = Brushes.White;
                topSphere.StrokeThickness = 1;
                Canvas.SetLeft(topSphere, (sceneObject.Transform.Position.x - topX - sceneObject.Transform.Scale.x / 2) * topZoom);
                Canvas.SetTop(topSphere, (sceneObject.Transform.Position.z - topY - sceneObject.Transform.Scale.z / 2) * topZoom);
                top.Children.Add(topSphere);

                //Right Canvas
                Ellipse rightSphere = new Ellipse();
                rightSphere.Width = sceneObject.Transform.Scale.z * rightZoom;
                rightSphere.Height = sceneObject.Transform.Scale.y * rightZoom;
                rightSphere.Stroke = Brushes.White;
                rightSphere.StrokeThickness = 1;
                Canvas.SetLeft(rightSphere,
                    (rightX - sceneObject.Transform.Position.z - sceneObject.Transform.Scale.z / 2) * rightZoom);
                Canvas.SetTop(rightSphere,
                    (rightY - sceneObject.Transform.Position.y - sceneObject.Transform.Scale.y / 2) * rightZoom);
                right.Children.Add(rightSphere);

                //Front Canvas
                Ellipse frontSphere = new Ellipse();
                frontSphere.Width = sceneObject.Transform.Scale.x * frontZoom;
                frontSphere.Height = sceneObject.Transform.Scale.y * frontZoom;
                frontSphere.Stroke = Brushes.White;
                frontSphere.StrokeThickness = 1;
                Canvas.SetLeft(frontSphere,
                    (sceneObject.Transform.Position.x - frontX - sceneObject.Transform.Scale.x / 2) * frontZoom);
                Canvas.SetTop(frontSphere,
                    (frontY - sceneObject.Transform.Position.y - sceneObject.Transform.Scale.y / 2) * frontZoom);
                front.Children.Add(frontSphere);
            }

            
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
            UpdateCanvas();

        }
    }
}
