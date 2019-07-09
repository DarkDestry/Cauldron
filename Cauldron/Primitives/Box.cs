using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Cauldron.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Cauldron.Primitives
{
    public class Box : IMesh
    {
        CldVector3[] vertices = new CldVector3[8];
        int[] vertexTriangles;
        CldVector3[] vertexNormals = new CldVector3[36];
        Vector2[] uvCoordinates = new Vector2[36];

        public Box()
        {
            SetupGeometry();
        }

        public void SetupGeometry()
        {


            vertices[0] = new CldVector3(0.5f, 0.5f, 0.5f);
            vertices[1] = new CldVector3(0.5f, 0.5f, -0.5f);
            vertices[2] = new CldVector3(0.5f, -0.5f, -0.5f);
            vertices[3] = new CldVector3(0.5f, -0.5f, 0.5f);
            vertices[4] = new CldVector3(-0.5f, 0.5f, 0.5f);
            vertices[5] = new CldVector3(-0.5f, 0.5f, -0.5f);
            vertices[6] = new CldVector3(-0.5f, -0.5f, -0.5f);
            vertices[7] = new CldVector3(-0.5f, -0.5f, 0.5f);


            vertexTriangles = new[]
            {
                0, 1, 2, 0, 2, 3, //Right Face
                0, 4, 5, 0, 5, 1, //Top Face
                0, 3, 7, 0, 7, 4, //Front Face
                4, 7, 6, 4, 6, 5, //Left Face
                6, 7, 3, 6, 3, 2, //Bottom Face
                5, 2, 1, 5, 6, 2 //Back Face
            };


            vertexNormals = new[]
            {
                //Right Face
                new CldVector3(1, 0, 0),
                new CldVector3(1, 0, 0),
                new CldVector3(1, 0, 0),
                new CldVector3(1, 0, 0),
                new CldVector3(1, 0, 0),
                new CldVector3(1, 0, 0),

                //Top Face
                new CldVector3(0, 1, 0),
                new CldVector3(0, 1, 0),
                new CldVector3(0, 1, 0),
                new CldVector3(0, 1, 0),
                new CldVector3(0, 1, 0),
                new CldVector3(0, 1, 0),

                //Front Face
                new CldVector3(0, 0, 1),
                new CldVector3(0, 0, 1),
                new CldVector3(0, 0, 1),
                new CldVector3(0, 0, 1),
                new CldVector3(0, 0, 1),
                new CldVector3(0, 0, 1),

                //Left Face
                new CldVector3(-1, 0, 0),
                new CldVector3(-1, 0, 0),
                new CldVector3(-1, 0, 0),
                new CldVector3(-1, 0, 0),
                new CldVector3(-1, 0, 0),
                new CldVector3(-1, 0, 0),

                //Bottom Face
                new CldVector3(0, -1, 0),
                new CldVector3(0, -1, 0),
                new CldVector3(0, -1, 0),
                new CldVector3(0, -1, 0),
                new CldVector3(0, -1, 0),
                new CldVector3(0, -1, 0),

                //Back Face
                new CldVector3(0, 0, -1),
                new CldVector3(0, 0, -1),
                new CldVector3(0, 0, -1),
                new CldVector3(0, 0, -1),
                new CldVector3(0, 0, -1),
                new CldVector3(0, 0, -1)
            };

            uvCoordinates = new[]
            {
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(0, 1),
                new Vector2(0, 0),
                new Vector2(1, 0)
            };
        }

        public VertexPositionNormalTexture[] GetVertexPositionNormalTexture()
        {
            VertexPositionNormalTexture[] vpnt = new VertexPositionNormalTexture[36];

            for (int i = 0; i < 36; i++)
            {
                vpnt[i].Position = vertices[vertexTriangles[i]];
                vpnt[i].Normal = vertexNormals[i];
                vpnt[i].TextureCoordinate = uvCoordinates[i];
            }

            return vpnt;
        }

        public VertexPositionNormalTexture[] GetModelVertexPositionNormalTexture(Transform transform)
        {
            VertexPositionNormalTexture[] vpnt = new VertexPositionNormalTexture[36];

            Matrix matrix = Matrix.CreateFromQuaternion(transform.Rotation);
            matrix *= Matrix.CreateScale(transform.Scale);
            matrix *= Matrix.CreateTranslation(transform.Position);

            for (int i = 0; i < 36; i++)
            {
                vpnt[i].Position = Vector3.Transform(vertices[vertexTriangles[i]], matrix);
                vpnt[i].Normal = vertexNormals[i];
                vpnt[i].TextureCoordinate = uvCoordinates[i];
            }

            return vpnt;

        }

    }
}
