using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron.Primitives
{
    public abstract class Mesh
    {
        public abstract VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform);
        public abstract VertexPositionNormalTexture[] GetVertexPositionNormalTexture();
    }
}
