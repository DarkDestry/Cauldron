using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using Cauldron.Core;
using GlmSharp;
using SharpGL;
using SharpGL.VertexBuffers;

namespace Cauldron.Primitives
{
    public class Icosphere : Mesh
    {
        private Dictionary<Int64, int> indexDictionary = new Dictionary<Int64, int>();
        List<CldVector3> Vertices = new List<CldVector3>(12);
        List<int[]> Faces = new List<int[]>();
        private int recursionLevel;

        public Icosphere(int recursionLevel)
        {
            this.recursionLevel = recursionLevel;
            SetupGeometry();
        }

        private int AddUnitVertex(CldVector3 v3)
        {
            float length = (float)Math.Sqrt(v3.X * v3.X + v3.Y * v3.Y + v3.Z * v3.Z);
            Vertices.Add(new CldVector3(v3.X / length, v3.Y / length, v3.Z / length));
            return Vertices.Count - 1;
        }

        public void SetupGeometry()
        {
            // create 12 vertices of a icosahedron
            float t = (float) ((1.0 + Math.Sqrt(5.0)) / 2.0);

            AddUnitVertex(new CldVector3(-1, t, 0));
            AddUnitVertex(new CldVector3(1, t, 0));
            AddUnitVertex(new CldVector3(-1, -t, 0));
            AddUnitVertex(new CldVector3(1, -t, 0));

            AddUnitVertex(new CldVector3(0, -1, t));
            AddUnitVertex(new CldVector3(0, 1, t));
            AddUnitVertex(new CldVector3(0, -1, -t));
            AddUnitVertex(new CldVector3(0, 1, -t));

            AddUnitVertex(new CldVector3(t, 0, -1));
            AddUnitVertex(new CldVector3(t, 0, 1));
            AddUnitVertex(new CldVector3(-t, 0, -1));
            AddUnitVertex(new CldVector3(-t, 0, 1));

            // 5 faces around point 0
            Faces.Add(new[]{0, 5, 11});
            Faces.Add(new[]{0, 1, 5});
            Faces.Add(new[]{0, 7, 1});
            Faces.Add(new[]{0, 10, 7});
            Faces.Add(new[]{0, 11, 10});

            // 5 adjacent faces 
            Faces.Add(new[]{1, 9, 5});
            Faces.Add(new[]{5, 4, 11});
            Faces.Add(new[]{11, 2, 10});
            Faces.Add(new[]{10, 6, 7});
            Faces.Add(new[]{7, 8, 1});

            // 5 faces around point 3
            Faces.Add(new[]{3, 4, 9});
            Faces.Add(new[]{3, 2, 4});
            Faces.Add(new[]{3, 6, 2});
            Faces.Add(new[]{3, 8, 6});
            Faces.Add(new[]{3, 9, 8});

            // 5 adjacent faces 
            Faces.Add(new[]{4, 5, 9});
            Faces.Add(new[]{2, 11, 4});
            Faces.Add(new[]{6, 10, 2});
            Faces.Add(new[]{8, 7, 6});
            Faces.Add(new[]{9, 1, 8});

            for (int i = 0; i < recursionLevel; i++)
            {
                List<int[]> SubdividedFaces = new List<int[]>(Faces.Count * 4);
                foreach (var tri in Faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePointIndex(tri[0], tri[1]);
                    int b = GetMiddlePointIndex(tri[1], tri[2]);
                    int c = GetMiddlePointIndex(tri[2], tri[0]);

                    SubdividedFaces.Add(new[] {tri[0], a, c});
                    SubdividedFaces.Add(new[] {tri[1], b, a});
                    SubdividedFaces.Add(new[] {tri[2], c, b});
                    SubdividedFaces.Add(new[] {a, b, c});
                }

                Faces = SubdividedFaces;
            }
        }

        private int GetMiddlePointIndex(int p1, int p2)
        {
            Int64 lower = Math.Min(p1, p2);
            Int64 higher = Math.Max(p1, p2);

            Int64 key = (lower << 32) + higher;
            if (indexDictionary.TryGetValue(key, out int index)) return index;

            CldVector3 point1 = Vertices[p1];
            CldVector3 point2 = Vertices[p2];
            CldVector3 middle = new CldVector3(
                (float) ((point1.X + point2.X) / 2.0),
                (float) ((point1.Y + point2.Y) / 2.0),
                (float) ((point1.Z + point2.Z) / 2.0));

            index = AddUnitVertex(middle);

            indexDictionary.Add(key, index);
            return index;
        }

        public override void GenerateGeometry(OpenGL gl)
        {
            vertexBufferArray = new VertexBufferArray();
            vertexBufferArray.Create(gl);
            vertexBufferArray.Bind(gl);

            VertexBuffer vb = new VertexBuffer();
            vb.Create(gl);
            vb.Bind(gl);
            vb.SetData(gl, 0, Vertices.SelectMany(v => ((vec3)v).ToArray()).ToArray(), false, 3);

            VertexBuffer nb = new VertexBuffer();
            nb.Create(gl);
            nb.Bind(gl);
            nb.SetData(gl, 1, Vertices.SelectMany(v => ((vec3)v).ToArray()).ToArray(), false, 3);

            IndexBuffer ib = new IndexBuffer();
            ib.Create(gl);
            ib.Bind(gl);
            ib.SetData(gl, Faces.SelectMany(v => v).Select(v => (ushort)v).ToArray());
        }

        public override void Draw(OpenGL gl)
        {
            vertexBufferArray.Bind(gl);

            gl.DrawElements(OpenGL.GL_TRIANGLES, Faces.Count*3, OpenGL.GL_UNSIGNED_SHORT, IntPtr.Zero);
        }
    }
}
