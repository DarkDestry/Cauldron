using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron.Primitives
{
    class Empty : IMesh
    {
        public VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform)
        {
            return null;
        }

        public VertexPositionNormalTexture[] GetVertexPositionNormalTexture()
        {
            return null;
        }
    }
}
