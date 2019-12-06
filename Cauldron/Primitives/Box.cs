using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;
using Cauldron.Core;
using GlmSharp;
using SharpGL;
using SharpGL.VertexBuffers;

namespace Cauldron.Primitives
{
    public class Box : Mesh
    {
        CldVector3[] vertices = new CldVector3[8];
        ushort[] vertexTriangles;
        CldVector3[] vertexNormals = new CldVector3[36];
        CldVector2[] uvCoordinates = new CldVector2[36];
        public int indicesLength = 36;

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


            vertexTriangles = new ushort[]
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
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 0),
                new CldVector2(1, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 1),
                new CldVector2(0, 0),
                new CldVector2(1, 0)
            };
        }

        public VertexBufferArray vertexBufferArray;

        public override void GenerateGeometry(OpenGL gl)
        {
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            VertexBuffer vb = new VertexBuffer();
            vb.Create(gl);
            vb.Bind(gl);
            vb.SetData(gl, 0, vertices.SelectMany(v => ((vec3) v).ToArray()).ToArray(), false, 3);

            IndexBuffer ib = new IndexBuffer();
            ib.Create(gl);
            ib.Bind(gl);
            ib.SetData(gl, vertexTriangles);
        }

        public override void Draw(OpenGL gl)
        {
            vertexBufferArray.Bind(gl);

            gl.DrawElements(OpenGL.GL_TRIANGLES, 36, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
        }
    }
}
