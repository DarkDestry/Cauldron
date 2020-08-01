using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using SlimDX.Direct3D9;
using Cauldron.Core;
using Cauldron.Primitives;
using GlmSharp;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
using SharpGL.WPF;
using Box = Cauldron.Primitives.Box;
using Control = System.Windows.Controls.Control;
using Cursors = System.Windows.Input.Cursors;
using Grid = System.Windows.Controls.Grid;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Shader = SharpGL.Shaders.Shader;

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "Viewport_3D", Type = typeof(OpenGLControl))]
    [TemplatePart(Name = "Ether_Viewport", Type = typeof(Control_Ether_HwndHost))]
    [TemplatePart(Name = "Shader_Compiler_Error", Type = typeof(TextBlock))]
    [TemplatePart(Name = "SceneWindow", Type = typeof(Grid))]
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
        private OpenGLControl openGLControl;
        private Control_Ether_HwndHost hwndHostElement;
        private TextBlock shaderErrorLabel;
        private Grid sceneWindow;

        //Camera stuff
        private float cameraPitch = -30;
        private float cameraYaw = 150;
        private float distance = 20;
        private CldVector3 cameraFocusPoint = new CldVector3(0, 0, 0);

        private mat4 cameraTransform =>
            mat4.Translate(0,0,-distance) *
            mat4.RotateX(CldMath.DegToRad(-cameraPitch)) *
            mat4.RotateY(CldMath.DegToRad(-cameraYaw)) *
            mat4.Translate(-(vec3)cameraFocusPoint);

        private vec3 cameraUp => (vec3) (mat4.RotateY(CldMath.DegToRad(cameraYaw)) *
                                         mat4.RotateX(CldMath.DegToRad(cameraPitch)) * vec4.UnitY);
        private vec3 cameraRight => (vec3) (mat4.RotateY(CldMath.DegToRad(cameraYaw)) *
                                            mat4.RotateX(CldMath.DegToRad(cameraPitch)) * vec4.UnitX);

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
                 
            if (Template != null)
            {
                openGLControl = Template.FindName("Viewport_3D", this) as OpenGLControl;
                hwndHostElement = Template.FindName("Ether_Viewport", this) as Control_Ether_HwndHost;
                sceneWindow = Template.FindName("SceneWindow", this) as Grid;

                //TODO: Reference by name
                if (CommandArguments.TryGetArgument("ether"))
                {
                    sceneWindow.Children.RemoveAt(1);
                    return;
                }
                sceneWindow.Children.RemoveAt(2);

                openGLControl.OpenGLDraw += GL_Draw;
                openGLControl.OpenGLInitialized += GL_Init;
                openGLControl.Resized += OpenGlControlOnResized;
                openGLControl.MouseMove += Viewport_MouseMove;
                openGLControl.GotMouseCapture += Viewport_MouseMove;
                openGLControl.PreviewMouseDown += MouseMiddleButtonDown;
                openGLControl.RenderContextType = RenderContextType.FBO;
                openGLControl.MouseWheel += OpenGlControlOnMouseWheel;
                openGLControl.DrawFPS = true;

                //HACK: To override the private timer field to allow for render to have higher priority than input.
                var prop = openGLControl.GetType().GetField("timer",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                prop.SetValue(openGLControl, new DispatcherTimer(DispatcherPriority.Send));

                shaderErrorLabel = Template.FindName("Shader_Compiler_Error", this) as TextBlock;

                Hierarchy.FocusChangedEvent += o => cameraFocusPoint = o.Transform.Position;
            }
        }

        private void OpenGlControlOnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double delta = -e.Delta;
            distance += (float)(CldMath.Clamp(delta / 200, -1, 1) *
                         CldMath.Clamp(Math.Pow(distance / 10, 2), 0, 30));
        }

        private void OpenGlControlOnResized(object sender, OpenGLRoutedEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Create the projection matrix for the screen size.
            CreateProjectionMatrix(gl, (float)ActualWidth, (float)ActualHeight);
        }

        private void GL_Init(object sender, OpenGLRoutedEventArgs args)
        {
            OpenGL gl = args.OpenGL;

            gl.Enable(OpenGL.GL_DEPTH_TEST);

            float[] global_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1.0f };
            float[] light0pos = new float[] { 0.0f, 5.0f, 10.0f, 1.0f };
            float[] light0ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };

            float[] lmodel_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, lmodel_ambient);

            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            gl.ShadeModel(OpenGL.GL_SMOOTH);

            grid.GenerateGeometry(gl);

            ShaderStore.CompileShaders(gl);
        }

        float rotation = 0;
        private Box b;
        private float theta = 0;
        Primitives.Grid grid = new Primitives.Grid();

        private void GL_Draw(object sender, OpenGLRoutedEventArgs args)
        {

            OpenGL gl = args.OpenGL;


            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                if (!sceneObject.MeshRenderer.Mesh.GeometryGenerated)
                    sceneObject.MeshRenderer.Mesh.GenerateGeometry(gl);
            }

#if DEBUG
            #region Hot Reload Shaders
            ShaderStore.CompileShadersHotReload(gl);
            shaderErrorLabel.Text = "\n" + ShaderStore.GetShaderErrors();
            shaderErrorLabel.Visibility = (shaderErrorLabel.Text) == "\n" ? Visibility.Hidden : Visibility.Visible;
            #endregion
#endif

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
            gl.ClearColor((float)37/255, (float)37 /255, (float)38 /255, 1);

            ShaderProgram program = ShaderStore.programs["unlitColor"];

            program.Push(gl, null);

            //  Set the variables for the shader program.
            gl.Uniform3(program.GetUniformLocation("AmbientMaterial"), 0.04f, 0.04f, 0.04f);
            gl.Uniform3(program.GetUniformLocation("SpecularMaterial"), 0.5f, 0.5f, 0.5f);
            gl.Uniform1(program.GetUniformLocation("Shininess"), 50f);

            //  Set the light position.
            gl.Uniform3(program.GetUniformLocation("LightPosition"), 0.25f, 0.25f, 1f);

            //  Set the matrices.
            gl.UniformMatrix4(program.GetUniformLocation("Projection"), 1, false, projectionMatrix.ToArray());
            gl.UniformMatrix4(program.GetUniformLocation("View"), 1, false, cameraTransform.ToArray());
            gl.UniformMatrix3(program.GetUniformLocation("NormalMatrix"), 1, false, normalMatrix.ToArray());

            //  Draw Grid
            grid.Draw(gl, program);

            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                GLColor col = sceneObject.MeshRenderer.Color;
                gl.Uniform3(program.GetUniformLocation("DiffuseMaterial"), col.R, col.G, col.B);

                gl.UniformMatrix4(program.GetUniformLocation("Model"), 1, false, sceneObject.Transform.Matrix.ToArray());
                sceneObject.MeshRenderer.Mesh.Draw(gl);
            }

            program.Pop(gl, null);
        }

        private bool mmDown;
        private bool panning;
        private bool orbiting;
        private Point cursorPos;

        private void Viewport_MouseMove(object sender, MouseEventArgs e)
        {
            OpenGLControl viewport = sender as OpenGLControl;

            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                if (!mmDown) cursorPos = Mouse.GetPosition(viewport);

                mmDown = true;
                if (Keyboard.IsKeyDown(Key.LeftShift)) panning = true;
                else orbiting = true;
            }
            else if (e.MiddleButton == MouseButtonState.Released)
            {
                mmDown = false;
                panning = false;
                orbiting = false;

                Mouse.OverrideCursor = null;
                Mouse.Capture(null);
            }

            if (orbiting)
            {
                Vector delta = e.GetPosition(viewport) - cursorPos;

                cursorPos = e.GetPosition(viewport);

                cameraPitch += (float)-delta.Y / 2;
                cameraYaw += (float)-delta.X / 2;
            }

            if (panning)
            {
                Vector delta = e.GetPosition(viewport) - cursorPos;

                cursorPos = e.GetPosition(viewport);
                if (delta.X != 0)
                {
                    bool b = true;
                }
                cameraFocusPoint += (CldVector3)(cameraRight * (float) -delta.X / 30);
                cameraFocusPoint += (CldVector3)(cameraUp * (float) delta.Y / 30);
            }

        }

        private void MouseMiddleButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.MiddleButton == MouseButtonState.Pressed)
            {
                e.Handled = true;
                Mouse.Capture(sender as IInputElement);
                Mouse.OverrideCursor = Cursors.SizeAll;
            }
        }

        public void CreateProjectionMatrix(OpenGL gl, float screenWidth, float screenHeight)
        {
            //  Create the projection matrix for our screen size.
            const float S = 0.46f;
            float H = S * screenHeight / screenWidth;
            projectionMatrix = mat4.Frustum(-S, S, -H, H, 1, 100);

            //  When we do immediate mode drawing, OpenGL needs to know what our projection matrix
            //  is, so set it now.
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.MultMatrix(projectionMatrix.ToArray());
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
        }


        private mat4 projectionMatrix = mat4.Identity;
        private mat3 normalMatrix = mat3.Identity;
    }
}
