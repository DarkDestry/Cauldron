using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using SharpGL;
using SharpGL.VertexBuffers;

namespace Cauldron.Primitives
{
    public abstract class Mesh
    {
        protected VertexBufferArray vertexBufferArray;

        public bool GeometryGenerated => vertexBufferArray != null;

        public abstract void Draw(OpenGL gl);
        public abstract void GenerateGeometry(OpenGL gl);
    }
}
