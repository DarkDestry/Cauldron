using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Primitives;
using SharpGL.SceneGraph;

namespace Cauldron.Core
{
    public class MeshRenderer
    {
        private GLColor color;
        private Mesh mesh;

        public MeshRenderer()
        {
            Color = new GLColor(0,0,0,1);
            Mesh = new Empty();
        }

        public GLColor Color
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
