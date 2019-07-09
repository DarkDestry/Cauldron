using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron.Core
{
    public interface IMesh
    {
        VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform);
        VertexPositionNormalTexture[] GetVertexPositionNormalTexture();
    }
}
