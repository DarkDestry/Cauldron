using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public override VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform)
        {
            Matrix matrix = Matrix.CreateFromQuaternion(transform.Rotation);
            matrix *= Matrix.CreateScale(transform.Scale);
            matrix *= Matrix.CreateTranslation(transform.Position);

            VertexPositionNormalTexture[] vpnt = new VertexPositionNormalTexture[faces.Length];

            for (int i = 0; i < vpnt.Length; i++)
            {
                vpnt[i].Normal = normals[faces[i].Normal];
                vpnt[i].TextureCoordinate = textureCoords[faces[i].TextureCoord];
                vpnt[i].Position = Vector3.Transform(vertices[faces[i].Position], matrix);
            }

            return vpnt;
        }

        public override VertexPositionNormalTexture[] GetVertexPositionNormalTexture()
        {
            VertexPositionNormalTexture[] vpnt = new VertexPositionNormalTexture[faces.Length];

            for (int i = 0; i < vpnt.Length; i++)
            {
                vpnt[i].Normal = normals[faces[i].Normal];
                vpnt[i].TextureCoordinate = textureCoords[faces[i].TextureCoord];
                vpnt[i].Position = vertices[faces[i].Position];
            }

            return vpnt;
        }
    }
}
