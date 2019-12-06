using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron.Primitives
{
    public class Empty : Mesh
    {
        public override VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform)
        {
            return null;
        }

        public override VertexPositionNormalTexture[] GetVertexPositionNormalTexture()
        {
            return null;
        }
    }
}
