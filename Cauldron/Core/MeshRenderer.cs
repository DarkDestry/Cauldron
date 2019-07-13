using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Primitives;
using Microsoft.Xna.Framework;
using SharpDX.Direct3D9;

namespace Cauldron.Core
{
    public class MeshRenderer
    {
        private Color color;
        private IMesh mesh;

        public MeshRenderer()
        {
            Color = Color.Black;
            Mesh = new Empty();
        }

        public Color Color
        {
            get => color;
            set => color = value;
        }

        public IMesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
    }
}
