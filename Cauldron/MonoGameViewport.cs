using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
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

        private void Hierarchy_FocusChangedEvent(Hierarchy.SceneObject obj)
        {
            cameraFocusPoint = obj.Transform.Position;
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
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
            float deltaTime = (float)time.ElapsedGameTime.Milliseconds/1000;

            HandleInput(deltaTime);

            GraphicsDevice.Clear(new Color(new Vector3(0.14f)));


            effect.View = Matrix.CreateLookAt(cameraPosition, cameraFocusPoint, cameraUp);

            float aspectRatio = (float) (ActualWidth / ActualHeight);
            float fieldOfView = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

            //effect.VertexColorEnabled = true;

            int gridLineCount = 8;

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
                new VertexPositionNormalTexture(Vector3.Forward * 10, Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Forward * -10, Vector3.Up, Vector2.Zero)
            };
            var rightLine = new[]
            {
                new VertexPositionNormalTexture(Vector3.Right * -10, Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Right * 10, Vector3.Up, Vector2.Zero)
            };
            var upLine = new[]
            {
                new VertexPositionNormalTexture(Vector3.Up * -10, Vector3.Up, Vector2.Zero),
                new VertexPositionNormalTexture(Vector3.Up * 10, Vector3.Up, Vector2.Zero)
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

                effect.DiffuseColor = new Vector3(0.3f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    worldGrid,
                    0,
                    worldGrid.Length/2
                );

                effect.DiffuseColor = new Vector3(0.1f, 0.1f, 0.6f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    forwardLine,
                    0,
                    1
                );

                effect.DiffuseColor = new Vector3(0.6f, 0.1f, 0.1f); ;
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.LineList,
                    rightLine,
                    0,
                    1
                );

                //effect.DiffuseColor = new Vector3(0.1f, 0.6f, 0.1f); ;
                //pass.Apply();
                //GraphicsDevice.DrawUserPrimitives(
                //    PrimitiveType.LineList,
                //    upLine,
                //    0,
                //    1
                //);
            }

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
                int delta = _mouse.GetState().ScrollWheelValue - lastScrollWheelValue;
                lastScrollWheelValue = _mouse.GetState().ScrollWheelValue;

                distance -= delta / 10;
                distance = MathHelper.Clamp(distance, 1, float.MaxValue);
                System.Diagnostics.Trace.WriteLine(distance.ToString());
            }
        }
    }
}
