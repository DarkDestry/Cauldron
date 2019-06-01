using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SharpDX.Direct3D9;
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

            // must be called after the WpfGraphicsDeviceService instance was created
            base.Initialize();

            // content loading now possible
        }

        protected override void Update(GameTime time)
        {
            // every update we can now query the keyboard & mouse for our WpfGame
            var mouseState = _mouse.GetState();
            var keyboardState = _keyboard.GetState();
        }

        private float cameraPitch = 0, cameraYaw = 0;
        private Vector3 cameraPos = new CldVector3(0, 0, 30);
        private Vector3 cameraVector => Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(cameraYaw), MathHelper.ToRadians(cameraPitch), 0));
        private Vector3 cameraRight => Vector3.Normalize(Vector3.Cross(cameraVector, Vector3.Up));

        private float lastTotalTime = 0;

        protected override void Draw(GameTime time)
        {
            float deltaTime = (float)time.ElapsedGameTime.Milliseconds/1000;
            System.Diagnostics.Trace.WriteLine(deltaTime);

            HandleInput(deltaTime);

            GraphicsDevice.Clear(Color.Black);


            effect.View = Matrix.CreateLookAt(cameraPos, cameraPos+cameraVector, new CldVector3(0,1,0));

            float aspectRatio = (float) (ActualWidth / ActualHeight);
            float fieldOfView = MathHelper.PiOver4;
            float nearClipPlane = 1;
            float farClipPlane = 200;

            effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                fieldOfView, aspectRatio, nearClipPlane, farClipPlane);

            effect.EnableDefaultLighting();
            

            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                foreach (var sceneObject in Hierarchy.hierarchyObjectList)
                {
                    Box box = new Box(sceneObject.Transform);

                    GraphicsDevice.DrawUserPrimitives(
                        PrimitiveType.TriangleList,
                        box.GetVertexPositionNormalTexture(),
                        0,
                        12);


                }
            }

        }

        private Point pos;

        private void HandleInput(float deltaTime)
        {
            if (_mouse.GetState().RightButton == ButtonState.Pressed)
            {
                if (!_mouse.CaptureMouseWithin) pos = _mouse.GetState().Position;

                _mouse.CaptureMouseWithin = true;
            }
            else _mouse.CaptureMouseWithin = false;

            if (_mouse.CaptureMouseWithin)
            {
                Point delta = _mouse.GetState().Position - pos;

                pos = _mouse.GetState().Position;

                cameraPitch += (float)-delta.Y/2;
                cameraYaw += (float)-delta.X/2;

                if (_keyboard.GetState().IsKeyDown(Keys.W)) cameraPos += cameraVector * deltaTime * 2;
                if (_keyboard.GetState().IsKeyDown(Keys.S)) cameraPos -= cameraVector * deltaTime * 2;
                if (_keyboard.GetState().IsKeyDown(Keys.D)) cameraPos += cameraRight * deltaTime * 2;
                if (_keyboard.GetState().IsKeyDown(Keys.A)) cameraPos -= cameraRight * deltaTime * 2;
                if (_keyboard.GetState().IsKeyDown(Keys.Q)) cameraPos += Vector3.Up * deltaTime * 2;
                if (_keyboard.GetState().IsKeyDown(Keys.E)) cameraPos -= Vector3.Up * deltaTime * 2;
            }
        }
    }
}
