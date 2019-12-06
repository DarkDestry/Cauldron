using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Primitives;
using Microsoft.Xna.Framework;

namespace Cauldron.Core
{
    public class MeshRenderer
    {
        private Color color;
        private Mesh mesh;

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

        public Mesh Mesh
        {
            get => mesh;
            set => mesh = value;
        }
    }
}
