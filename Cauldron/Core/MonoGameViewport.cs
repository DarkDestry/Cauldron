using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Cauldron.Core;
using Cauldron.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SharpDX;
using SharpDX.DirectWrite;
using Box = Cauldron.Primitives.Box;
using Color = Microsoft.Xna.Framework.Color;
using Matrix = Microsoft.Xna.Framework.Matrix;
using Point = Microsoft.Xna.Framework.Point;
using PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace Cauldron
{
    public class MonoGameViewport : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private BasicEffect effect;
        private Effect outlineEffect;
        private Effect addEffect;
        private Dictionary<string, RenderTarget2D> renderTargets;

        private int screenHeight = 0, screenWidth = 0;
        private void CheckForScreenResize()
        {
            if (screenHeight != GraphicsDevice.Viewport.Height || screenWidth != GraphicsDevice.Viewport.Width)
            {
                renderTargets = new Dictionary<string, RenderTarget2D>();
                screenWidth = GraphicsDevice.Viewport.Width;
                screenHeight = GraphicsDevice.Viewport.Height;
            }
        }

        protected override void Initialize()
        {
            // must be initialized. required by Content loading and rendering (will add itself to the Services)
            // note that MonoGame requires this to be initialized in the constructor, while WpfInterop requires it to
            // be called inside Initialize (before base.Initialize())
            _graphicsDeviceManager = new WpfGraphicsDeviceService(this);

            // wpf and keyboard need reference to the host control in order to receive input
            // this means every WpfGame control will have it's own keyboard & mouse manager which will only react if the mouse is in the control
            _keyboard = new WpfKeyboard(this);
            _mouse = new WpfMouse(this);

            effect = new BasicEffect(GraphicsDevice);
            effect.EnableDefaultLighting();
            effect.SpecularColor = Vector3.Zero;

            Hierarchy.FocusChangedEvent += Hierarchy_FocusChangedEvent;

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            //Initialize renderTargets;
            renderTargets = new Dictionary<string, RenderTarget2D>();

            // content loading now possible
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = @".\Content\bin\";
            outlineEffect = Content.Load<Effect>(@"Shaders\Outline");
            addEffect = Content.Load<Effect>(@"Shaders\Add");
        }

        private void Hierarchy_FocusChangedEvent(Hierarchy.SceneObject obj)
        {
            cameraFocusPoint = obj.Transform.Position;
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
            float deltaTime = (float)time.ElapsedGameTime.Milliseconds / 1000;

            HandleInput(deltaTime);
        }

        private float cameraPitch = -30;
        private float cameraYaw = 150;
        private float distance = 20;
        private Vector3 cameraFocusPoint = new CldVector3(0,0,0);

        private Matrix cameraTransform => Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(cameraYaw), MathHelper.ToRadians(cameraPitch), 0);

        private Vector3 cameraForward => Vector3.Transform(Vector3.Forward, cameraTransform);
        private Vector3 cameraRight => Vector3.Transform(Vector3.Right, cameraTransform);
        private Vector3 cameraUp => Vector3.Transform(Vector3.Up, cameraTransform);

        private Vector3 cameraPosition => cameraFocusPoint - cameraForward * distance;

        private float lastTotalTime = 0;

        protected override void Draw(GameTime time)
        {
            CheckForScreenResize();

            const int gridLineCount = 16;

            VertexPositionNormalTexture[] worldGrid = new VertexPositionNormalTexture[(gridLineCount*2 +1)* 4];

            for (int i = -gridLineCount; i <= gridLineCount; i++)
            {
                if (i == 0) continue;
                worldGrid[(i + gridLineCount) * 4 + 0].Position = new Vector3(-gridLineCount, 0, i);
                worldGrid[(i + gridLineCount) * 4 + 1].Position = new Vector3(gridLineCount, 0, i);
                worldGrid[(i + gridLineCount) * 4 + 2].Position = new Vector3(i, 0, gridLineCount);
                worldGrid[(i + gridLineCount) * 4 + 3].Position = new Vector3(i, 0, -gridLineCount);
            }

            for (var i = 0; i < worldGrid.Length; i++)
            {
                worldGrid[i].Normal = Vector3.Up;
                worldGrid[i].TextureCoordinate = Vector2.Zero; 
            }

            var forwardLine = new[]
            {
                new VertexPositionNormalTexture(Vector3.Forward * (gridLineCount+1), Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Forward * -(gridLineCount+1), Vector3.Up, Vector2.Zero)
            };
            var rightLine = new[]
            {
                new VertexPositionNormalTexture(Vector3.Right * -(gridLineCount+1), Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Right * (gridLineCount+1), Vector3.Up, Vector2.Zero)
            };
            var upLine = new[]
            {
                new VertexPositionNormalTexture(Vector3.Up * -(gridLineCount+1), Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Up * (gridLineCount+1), Vector3.Up, Vector2.Zero)
            };
            var screenspaceQuad = new[]
            {
                new VertexPositionNormalTexture(-Vector3.Right - Vector3.Up,-Vector3.Forward, Vector2.UnitY),
                new VertexPositionNormalTexture(Vector3.Right + Vector3.Up,-Vector3.Forward,Vector2.UnitX),
                new VertexPositionNormalTexture(Vector3.Right - Vector3.Up,-Vector3.Forward,Vector2.One),
                new VertexPositionNormalTexture(-Vector3.Right - Vector3.Up,-Vector3.Forward,Vector2.UnitY),
                new VertexPositionNormalTexture(Vector3.Up - Vector3.Right,-Vector3.Forward,Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Right + Vector3.Up,-Vector3.Forward,Vector2.UnitX),
            };

            //Save original renderTarget
            RenderTarget2D wpfRenderTarget = (RenderTarget2D)GraphicsDevice.GetRenderTargets()[0].RenderTarget;
            SurfaceFormat format = GraphicsDevice.GetRenderTargets()[0].RenderTarget.Format;

            //=======START SCENE RENDER============
            RenderTarget2D sceneRenderTarget = GetRenderTarget("sceneRenderTarget");
            GraphicsDevice.SetRenderTarget(sceneRenderTarget);
            GraphicsDevice.Clear(new Color(new Vector3(0.14f)));
            
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;


            effect.View = Matrix.CreateLookAt(cameraPosition, cameraFocusPoint, cameraUp);

            float aspectRatio = (float)(ActualWidth / ActualHeight);
            float fieldOfView = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);
            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var sceneObject in Hierarchy.hierarchyObjectList)
                {
                    effect.DiffuseColor = sceneObject.MeshRenderer.Color.ToVector3();
                    pass.Apply();
                    //Box box = new Box(sceneObject.Transform);
                    IMesh mesh = sceneObject.MeshRenderer.Mesh;

                    GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        mesh.GetModelVertexPositionNormalTexture(sceneObject.Transform),
                        0,
                        mesh.GetModelVertexPositionNormalTexture(sceneObject.Transform).Length/3);

                }

                //grid plane lines
                effect.DiffuseColor = new Vector3(0.3f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    worldGrid,
                    0,
                    worldGrid.Length / 2
                );

                //Blue Z axis line
                effect.DiffuseColor = new Vector3(0.1f, 0.1f, 0.6f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    forwardLine,
                    0,
                    1
                );

                //Red X axis line
                effect.DiffuseColor = new Vector3(0.6f, 0.1f, 0.1f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    rightLine,
                    0,
                    1
                );

            }

            //============START OUTLINE RENDER============
            RenderTarget2D outlineRenderTarget = GetRenderTarget("outlineRenderTarget");
            Rectangle Rect = new Rectangle(0, 0, outlineRenderTarget.Width, outlineRenderTarget.Height);

            GraphicsDevice.SetRenderTarget(outlineRenderTarget);
            GraphicsDevice.Clear(new Color(0));

            outlineEffect.Parameters["World"].SetValue(Matrix.Identity);
            outlineEffect.Parameters["View"].SetValue(effect.View);
            outlineEffect.Parameters["Projection"].SetValue(effect.Projection);
            outlineEffect.Techniques[0].Passes[0].Apply();

            if (Hierarchy.selectedObject != null)
            {
                IMesh mesh = Hierarchy.selectedObject.MeshRenderer.Mesh;

                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    mesh.GetModelVertexPositionNormalTexture(Hierarchy.selectedObject.Transform),
                    0,
                    mesh.GetModelVertexPositionNormalTexture(Hierarchy.selectedObject.Transform).Length/3);
            }

            Texture2D pass1 = outlineRenderTarget;
            RenderTarget2D outlineResultRenderTarget = GetRenderTarget("outlineResultRenderTarget");
            GraphicsDevice.SetRenderTarget(outlineResultRenderTarget);
            GraphicsDevice.Clear(new Color(new Vector3(0)));

            outlineEffect.Parameters["colorMapTexture"].SetValue(pass1);
            float p1h = pass1.Height;
            float p1w = pass1.Width;
            Vector2[] offsets = new[]
            {
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 0),
                new Vector2(-1, -1),
                new Vector2(0, -1),
                new Vector2(1, -1),
                new Vector2(1, 0)
            };
            outlineEffect.Parameters["offsets"].SetValue(offsets);
            outlineEffect.Parameters["width"].SetValue((float)p1w);
            outlineEffect.Parameters["height"].SetValue((float)p1h);
            outlineEffect.Parameters["lineWidth"].SetValue(2.0f);
            outlineEffect.Techniques[0].Passes[1].Apply();

            GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                screenspaceQuad,
                0,
                2);

            //==============START COMBINATION PASS===========
            GraphicsDevice.SetRenderTarget(wpfRenderTarget);
            GraphicsDevice.Clear(new Color(new Vector3(0.14f)));

            addEffect.Parameters["tex1"].SetValue(outlineResultRenderTarget);
            addEffect.Parameters["tex2"].SetValue(sceneRenderTarget);
            addEffect.Techniques[0].Passes[0].Apply();

            GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                screenspaceQuad,
                0,
                2);

            //int[] rtData = new int[wpfRenderTarget.Width * wpfRenderTarget.Height];

            //sceneRenderTarget.GetData(rtData);
            //wpfRenderTarget.SetData(rtData);
        }

        private RenderTarget2D GetRenderTarget(string renderTargetKey)
        {
            if (!renderTargets.ContainsKey(renderTargetKey))
                renderTargets[renderTargetKey] = new RenderTarget2D(
                    GraphicsDevice, 
                    screenWidth, 
                    screenHeight, 
                    false, 
                    SurfaceFormat.Bgr32, 
                    DepthFormat.Depth16);

            return renderTargets[renderTargetKey];
        }

        private Point pos;
        private int lastScrollWheelValue;
        private bool orbiting;
        private bool panning;

        private void HandleInput(float deltaTime)
        {
            if (_mouse.GetState().MiddleButton == ButtonState.Pressed)
            {
                if (!_mouse.CaptureMouseWithin) pos = _mouse.GetState().Position;

                _mouse.CaptureMouseWithin = true;
                if (_keyboard.GetState().IsKeyDown(Keys.LeftShift)) panning = true;
                else orbiting = true;
            }
            else
            {
                _mouse.CaptureMouseWithin = false;
                panning = false;
                orbiting = false;
            }

            if (orbiting)
            {
                Point delta = _mouse.GetState().Position - pos;

                pos = _mouse.GetState().Position;

                cameraPitch += (float) -delta.Y / 2;
                cameraYaw += (float)-delta.X / 2;
            }

            if (panning)
            {
                Point delta = _mouse.GetState().Position - pos;

                pos = _mouse.GetState().Position;

                cameraFocusPoint += cameraRight * (float) -delta.X / 30;
                cameraFocusPoint += cameraUp * (float) delta.Y / 30;
            }

            if (lastScrollWheelValue != _mouse.GetState().ScrollWheelValue)
            {
                float delta = _mouse.GetState().ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = _mouse.GetState().ScrollWheelValue;
                delta = -delta;

                //scrollValue -= (float)delta / 500;
                //scrollValue = MathHelper.Clamp(scrollValue, 1, float.MaxValue);

                //distance = scrollValue * scrollValue * scrollValue * scrollValue / 81;

                distance += MathHelper.Clamp(delta/200, -1,1) * MathHelper.Clamp((float)Math.Pow(distance/10,2),0,30);
                //distance += MathHelper.Clamp(delta/200, -1,1) * MathHelper.Clamp((float)Math.Exp(distance/10 - 2),0,100);
                
            }
        }
    }
}
