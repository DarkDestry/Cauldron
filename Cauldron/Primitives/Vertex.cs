using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Primitives
{
    public struct Vertex
    {
        public Vertex(int position, int normal, int textureCoord)
        {
            this.Position = position;
            this.Normal = normal;
            this.TextureCoord = textureCoord;
        }

        public int Position;
        public int Normal;
        public int TextureCoord;
    }
}
