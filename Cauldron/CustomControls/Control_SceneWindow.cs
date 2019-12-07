using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cauldron.Core;
using Cauldron.Primitives;
using GlmSharp;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Shaders;
using SharpGL.WPF;
using Shader = SharpGL.Shaders.Shader;

namespace Cauldron.CustomControls
{
    [TemplatePart(Name = "Canvas_Front", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Right", Type = typeof(Canvas))]
    [TemplatePart(Name = "Canvas_Top", Type = typeof(Canvas))]
    [TemplatePart(Name = "Viewport_3D", Type = typeof(OpenGLControl))]
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

        //Camera stuff
        private float cameraPitch = -30;
        private float cameraYaw = 150;
        private float distance = 20;
        private CldVector3 cameraFocusPoint = new CldVector3(0, 0, 0);

        private mat4 cameraTransform => mat4.Translate(cameraFocusPoint) * 
            mat4.Translate(0,0,-distance) *
            mat4.RotateX((float) CldMath.DegToRad(-cameraPitch)) *
            mat4.RotateY((float) CldMath.DegToRad(-cameraYaw));

//        private double[,] cameraTransform => CldMatrix.MatrixFromPitchYawRoll(CldMath.DegToRad(cameraYaw), CldMath.DegToRad(cameraPitch), 0);


//        private CldVector3 cameraForward => CldVector3.Transform(CldVector3.Forward, cameraTransform);
//        private CldVector3 cameraRight => CldVector3.Transform(CldVector3.Right, cameraTransform);
//        private CldVector3 cameraUp => CldVector3.Transform(CldVector3.Up, cameraTransform);

//        private CldVector3 cameraPosition => cameraFocusPoint - cameraForward * distance;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
                 
            if (Template != null)
            {
                openGLControl = Template.FindName("Viewport_3D", this) as OpenGLControl;
                openGLControl.OpenGLDraw += GL_Draw;
                openGLControl.OpenGLInitialized += GL_Init;
                openGLControl.Resized += OpenGlControlOnResized;
                openGLControl.RenderContextType = RenderContextType.FBO;
            }
        }

        private void OpenGlControlOnResized(object sender, OpenGLEventArgs args)
        {
            //  Get the OpenGL instance.
            var gl = args.OpenGL;

            //  Create the projection matrix for the screen size.
            CreateProjectionMatrix(gl, (float)ActualWidth, (float)ActualHeight);
        }

        ShaderProgram program = new ShaderProgram();

        private void GL_Init(object sender, OpenGLEventArgs args)
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
            
            //  Create a vertex shader.
            VertexShader vertexShader = new VertexShader();
            vertexShader.CreateInContext(gl);
            vertexShader.SetSource(ResourceLoader.LoadEmbeddedTextFile(@"Shaders\color.vert"));

            //  Create a fragment shader.
            FragmentShader fragmentShader = new FragmentShader();
            fragmentShader.CreateInContext(gl);
            fragmentShader.SetSource(ResourceLoader.LoadEmbeddedTextFile(@"Shaders\color.frag"));

            //  Compile them both.
            vertexShader.Compile();
            fragmentShader.Compile();

            //  Build a program.
            program.CreateInContext(gl);

            //  Attach the shaders.
            program.AttachShader(vertexShader);
            program.AttachShader(fragmentShader);
            program.Link();

            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                sceneObject.MeshRenderer.Mesh.GenerateGeometry(gl);
            }
        }

        float rotation = 0;
        private Box b;
        private float theta = 0;

        private void GL_Draw(object sender, OpenGLEventArgs args)
        {

            OpenGL gl = args.OpenGL;

#if DEBUG
            string vert, frag;
            bool vertChanged = ResourceLoader.LoadTextFileIfChanged(@"Shaders\color.vert", out vert);
            bool fragChanged = ResourceLoader.LoadTextFileIfChanged(@"Shaders\color.frag", out frag);

            if (vertChanged || fragChanged)
            {
                program = new ShaderProgram();
                
                VertexShader vertexShader = new VertexShader();
                vertexShader.CreateInContext(gl);
                vertexShader.SetSource(vert);

                //  Create a fragment shader.
                FragmentShader fragmentShader = new FragmentShader();
                fragmentShader.CreateInContext(gl);
                fragmentShader.SetSource(frag);

                vertexShader.Compile();
                fragmentShader.Compile();

                if ((bool) !vertexShader.CompileStatus) MessageBox.Show(Application.Current.MainWindow, vertexShader.InfoLog);
                if ((bool) !fragmentShader.CompileStatus) MessageBox.Show(Application.Current.MainWindow, fragmentShader.InfoLog);

                program.CreateInContext(gl);

                program.AttachShader(vertexShader);
                program.AttachShader(fragmentShader);

                program.Link();
            }


#endif

            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT | OpenGL.GL_STENCIL_BUFFER_BIT);
            gl.ClearColor((float)37/255, (float)37 /255, (float)38 /255, 1);

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

            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                GLColor col = sceneObject.MeshRenderer.Color;
                gl.Uniform3(program.GetUniformLocation("DiffuseMaterial"), col.R, col.G, col.B);

                gl.UniformMatrix4(program.GetUniformLocation("Model"), 1, false, sceneObject.Transform.Matrix.ToArray());
                sceneObject.MeshRenderer.Mesh.Draw(gl);
            }

            program.Pop(gl, null);
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

        public void CreateModelviewAndNormalMatrix(float rotationAngle)
        {
            //  Create the modelview and normal matrix. We'll also rotate the scene
            //  by the provided rotation angle, which means things that draw it 
            //  can make the scene rotate easily.
            mat4 rotation = mat4.Rotate(rotationAngle, new vec3(0, 1, 0));
            mat4 translation = mat4.Translate(new vec3(0, 0, -4));
            modelviewMatrix = translation * rotation;
            normalMatrix = new mat3(modelviewMatrix);
        }

        private mat4 modelviewMatrix = mat4.Identity;
        private mat4 projectionMatrix = mat4.Identity;
        private mat3 normalMatrix = mat3.Identity;
    }
}
