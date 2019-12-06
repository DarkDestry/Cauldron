using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using SharpGL;

namespace Cauldron.Primitives
{
    public class Empty : Mesh
    {
        public override void GenerateGeometry(OpenGL gl)
        {
            throw new NotImplementedException();
        }

        public override void Draw(OpenGL gl) { }
    }
}
