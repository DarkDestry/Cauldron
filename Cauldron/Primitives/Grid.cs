using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlmSharp;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Shaders;
using SharpGL.VertexBuffers;

namespace Cauldron.Primitives
{

    class Grid
    {
        private const int gridCount = 10;
        private const int vertexCount = gridCount * 8 + 4;
        CldVector3[] vertices;
        ushort[] indices = new ushort[vertexCount];

        public Grid()
        {
            List<CldVector3> Vertices = new List<CldVector3>();
            for (int i = -gridCount; i <= gridCount; i++)
            {
                if (i == 0) continue;
                Vertices.Add((CldVector3.Forward * -gridCount) + (CldVector3.Right * i));
                Vertices.Add(CldVector3.Forward * gridCount + CldVector3.Right * i);
                Vertices.Add(CldVector3.Right * -gridCount + CldVector3.Forward * i);
                Vertices.Add(CldVector3.Right * gridCount + CldVector3.Forward * i);
            }
            Vertices.Add(CldVector3.Forward * gridCount);
            Vertices.Add(CldVector3.Forward * -gridCount);
            Vertices.Add(CldVector3.Right * gridCount);
            Vertices.Add(CldVector3.Right * -gridCount);
            
            vertices = Vertices.ToArray();

            for (int j = 0; j < indices.Length; j++)
            {
                indices[j] = (ushort) j;
            }
        }

        public void Draw(OpenGL gl, ShaderProgram program)
        {
            vertexBufferArray.Bind(gl);

            gl.UniformMatrix4(program.GetUniformLocation("Model"), 1, false, mat4.Identity.ToArray());

            gl.Uniform3(program.GetUniformLocation("DiffuseMaterial"), 0.3f, 0.3f, 0.3f);
            gl.DrawElements(OpenGL.GL_LINES, vertexCount -4, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);

            gl.Uniform3(program.GetUniformLocation("DiffuseMaterial"), 0.1f, 0.1f, 0.6f);
            gl.DrawElements(OpenGL.GL_LINES, 2, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero + sizeof(ushort) * (vertexCount-4));

            gl.Uniform3(program.GetUniformLocation("DiffuseMaterial"), 0.6f, 0.1f, 0.1f);
            gl.DrawElements(OpenGL.GL_LINES, 2, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero + sizeof(ushort) * (vertexCount-2));
        }

        private VertexBufferArray vertexBufferArray;
        public void GenerateGeometry(OpenGL gl)
        {
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            VertexBuffer vb = new VertexBuffer();
            vb.Create(gl);
            vb.Bind(gl);
            vb.SetData(gl, 0, vertices.SelectMany(v => ((vec3)v).ToArray()).ToArray(), false, 3);

            IndexBuffer ib = new IndexBuffer();
            ib.Create(gl);
            ib.Bind(gl);
            ib.SetData(gl, indices);
        }
    }
}
