using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SharpDX.DirectWrite;
using Box = Cauldron.Primitives.Box;
using Point = Microsoft.Xna.Framework.Point;
using PrimitiveType = Microsoft.Xna.Framework.Graphics.PrimitiveType;

namespace Cauldron
{
    public class MonoGameViewport : WpfGame
    {
        private IGraphicsDeviceService _graphicsDeviceManager;
        private WpfKeyboard _keyboard;
        private WpfMouse _mouse;
        private BasicEffect effect;
        private Effect outlineEffect;

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

            // content loading now possible
        }

        protected override void LoadContent()
        {
            Content.RootDirectory = @".\Content\bin\";
            outlineEffect = Content.Load<Effect>(@"Shaders\Outline");
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

        private float cameraPitch;
        private float cameraYaw;
        private float distance = 30;
        private Vector3 cameraFocusPoint = new CldVector3(0,0,0);

        private Matrix cameraTransform => Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(cameraYaw), MathHelper.ToRadians(cameraPitch), 0);

        private Vector3 cameraForward => Vector3.Transform(Vector3.Forward, cameraTransform);
        private Vector3 cameraRight => Vector3.Transform(Vector3.Right, cameraTransform);
        private Vector3 cameraUp => Vector3.Transform(Vector3.Up, cameraTransform);

        private Vector3 cameraPosition => cameraFocusPoint - cameraForward * distance;

        private float lastTotalTime = 0;

        protected override void Draw(GameTime time)
        {

            GraphicsDevice.Clear(new Color(new Vector3(0.14f)));


            effect.View = Matrix.CreateLookAt(cameraPosition, cameraFocusPoint, cameraUp);

            float aspectRatio = (float) (ActualWidth / ActualHeight);
            float fieldOfView = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

            //effect.VertexColorEnabled = true;

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
                new VertexPositionNormalTexture(Vector3.Zero,Vector3.Forward,Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Right,Vector3.Forward,Vector2.UnitX),
                new VertexPositionNormalTexture(Vector3.Right + Vector3.Up,Vector3.Forward,Vector2.One),
                new VertexPositionNormalTexture(Vector3.Zero,Vector3.Forward,Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Right + Vector3.Up,Vector3.Forward,Vector2.One),
                new VertexPositionNormalTexture(Vector3.Up,Vector3.Forward,Vector2.UnitY)
            };

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var sceneObject in Hierarchy.hierarchyObjectList)
                {
                    effect.DiffuseColor = sceneObject.Color.ToVector3();
                    pass.Apply();
                    Box box = new Box(sceneObject.Transform);

                    GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        box.GetVertexPositionNormalTexture(),
                        0,
                        12);

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

            RenderTarget2D wpfRenderTarget = (RenderTarget2D) GraphicsDevice.GetRenderTargets()[0].RenderTarget;
            RenderTarget2D outlineRenderTarget = new RenderTarget2D(GraphicsDevice, wpfRenderTarget.Width, wpfRenderTarget.Height);
            Rectangle Rect = new Rectangle(0, 0, outlineRenderTarget.Width, outlineRenderTarget.Height);

            GraphicsDevice.SetRenderTarget(outlineRenderTarget);

            outlineEffect.Parameters["World"].SetValue(Matrix.Identity);
            outlineEffect.Parameters["View"].SetValue(effect.View);
            outlineEffect.Parameters["Projection"].SetValue(effect.Projection);
            outlineEffect.Techniques[0].Passes[0].Apply();
            foreach (var sceneObject in Hierarchy.hierarchyObjectList)
            {
                Box box = new Box(sceneObject.Transform);

                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleList,
                    box.GetVertexPositionNormalTexture(),
                    0,
                    12);

            }

            Texture2D pass1 = outlineRenderTarget;

            GraphicsDevice.SetRenderTarget(wpfRenderTarget);

            outlineEffect.Parameters["colorMapTexture"].SetValue(pass1);
            Vector2[] offsets = new[]
            {
                new Vector2(1 / pass1.Width, 1 / pass1.Height),
                new Vector2(0 / pass1.Width, 1 / pass1.Height),
                new Vector2(-1 / pass1.Width, 1 / pass1.Height),
                new Vector2(-1 / pass1.Width, 0 / pass1.Height),
                new Vector2(-1 / pass1.Width, -1 / pass1.Height),
                new Vector2(0 / pass1.Width, -1 / pass1.Height),
                new Vector2(1 / pass1.Width, -1 / pass1.Height),
                new Vector2(1 / pass1.Width, 0 / pass1.Height)
            };
            outlineEffect.Parameters["offsets"].SetValue(offsets);
            outlineEffect.Techniques[0].Passes[1].Apply();

            SpriteBatch sb = new SpriteBatch(GraphicsDevice);

            GraphicsDevice.DrawUserPrimitives(
                PrimitiveType.TriangleList,
                screenspaceQuad,
                0,
                2);

            sb.Begin(0, BlendState.AlphaBlend, null, null, null, outlineEffect);
            sb.Draw(pass1, Vector2.Zero, Rect, Color.White);
            sb.End();


        }

        private Point pos;
        private float scrollValue = 10;
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
                int delta = _mouse.GetState().ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = _mouse.GetState().ScrollWheelValue;

                scrollValue -= (float)delta / 500;
                scrollValue = MathHelper.Clamp(scrollValue, 1, float.MaxValue);

                distance = scrollValue * scrollValue * scrollValue * scrollValue / 81;
                System.Diagnostics.Trace.WriteLine(distance.ToString());
            }
        }
    }
}
