using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using SharpGL;

namespace Cauldron.Primitives
{
    public class Model : Mesh
    {
        public Model(CldVector3[] vertices, CldVector3[] textureCoords, CldVector3[] normals, Vertex[] faces)
        {
            this.vertices = vertices;
            this.textureCoords = textureCoords;
            this.normals = normals;
            this.faces = faces;
        }

        private CldVector3[] vertices;
        private CldVector3[] textureCoords;
        private CldVector3[] normals;
        private Vertex[] faces;

        public override void GenerateGeometry(OpenGL gl)
        {
            throw new NotImplementedException();
        }

        public override void Draw(OpenGL gl)
        {
            throw new NotImplementedException();
        }
    }
}
