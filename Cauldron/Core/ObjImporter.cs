using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Primitives;

namespace Cauldron.Core
{
    public static class ObjImporter
    {
        public static Model Import(string path)
        {
            //TODO: Validation

            List<CldVector3> vertices = new List<CldVector3>();
            List<CldVector3> textureCoords = new List<CldVector3>();
            List<CldVector3> normals = new List<CldVector3>();
            List<Vertex> faces = new List<Vertex>();

            using (StreamReader sr = new StreamReader(path))
            {
                string s = sr.ReadLine();
                string[] entry = s.Split(' ');
                switch (entry[0])
                {
                    case "v":
                        vertices.Add(new CldVector3(float.Parse(entry[1]), float.Parse(entry[2]), float.Parse(entry[3])));
                        break;
                    case "vt":
                        textureCoords.Add(new CldVector3(float.Parse(entry[1]), float.Parse(entry[2])));
                        break;
                    case "n":
                        normals.Add(new CldVector3(float.Parse(entry[1]), float.Parse(entry[2]), float.Parse(entry[3])));
                        break;
                    case "f":
                        for (int i = 1; i < entry.Length; i++)
                        {
                            int[] vertProperties = entry[i].Split('/').Select(StringToInt).ToArray();
                            faces.Add(new Vertex(vertProperties[0], vertProperties[2], vertProperties[1]));
                        }
                        break;
                }
            }

            return new Model(vertices.ToArray(), textureCoords.ToArray(), normals.ToArray(), faces.ToArray());
        }

        private static int StringToInt(string s)
        {
            int x;
            if (int.TryParse(s, out x)) return x;
            else return 0;
        }
    }
}
