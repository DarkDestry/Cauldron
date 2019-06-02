using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron
{
    class SolidColorTexture : Texture2D
    {
        private Color _color;
        // Gets or sets the color used to create the texture
        public Color Color
        {
            get { return _color; }
            set
            {
                if (value != _color)
                {
                    _color = value;
                    SetData(new[] { _color });
                }
            }
        }


        public SolidColorTexture(GraphicsDevice graphicsDevice, Color color) : base(graphicsDevice, 1, 1)
        {
            Color = color;
        }

        public SolidColorTexture(GraphicsDevice graphicsDevice, bool mipmap, SurfaceFormat format, Color color) : base(graphicsDevice, 1, 1, mipmap, format)
        {
            Color = color;
        }

        public SolidColorTexture(GraphicsDevice graphicsDevice, bool mipmap, SurfaceFormat format, int arraySize, Color color) : base(graphicsDevice, 1, 1, mipmap, format, arraySize)
        {
            Color = color;
        }

        protected SolidColorTexture(GraphicsDevice graphicsDevice, bool mipmap, SurfaceFormat format, SurfaceType type, bool shared, int arraySize, Color color) : base(graphicsDevice, 1, 1, mipmap, format, type, shared, arraySize)
        {
            Color = color;
        }
    }
}
