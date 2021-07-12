using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using SharpGL;
using SharpGL.SceneGraph.Shaders;

namespace Cauldron.Core
{
    public static class ShaderStore
    {
        public static Dictionary<string, ShaderProgram> programs = new Dictionary<string, ShaderProgram>();
        private static Dictionary<string, string> shaderErrors = new Dictionary<string, string>();

        //TODO: detect list of shaders
        private static string[] shaderNames = new[]
        {
            "unlitColor",
            "color"
        };

        public static void CompileShaders(OpenGL gl)
        {

            foreach (var name in shaderNames)
            {
                ShaderProgram program = new ShaderProgram();
                string shaderError = "";

                //  Create a vertex shader.
                VertexShader vertexShader = new VertexShader();
                vertexShader.CreateInContext(gl);
                vertexShader.SetSource(ResourceLoader.LoadEmbeddedTextFile($"Shaders\\{name}.vert"));

                //  Create a fragment shader.
                FragmentShader fragmentShader = new FragmentShader();
                fragmentShader.CreateInContext(gl);
                fragmentShader.SetSource(ResourceLoader.LoadEmbeddedTextFile($"Shaders\\{name}.frag"));

                //  Compile them both.
                vertexShader.Compile();
                fragmentShader.Compile();

                if ((bool)!vertexShader.CompileStatus) shaderError += "color.vert: " + vertexShader.InfoLog + "\n\n";
                if ((bool)!fragmentShader.CompileStatus) shaderError += "color.frag: " + fragmentShader.InfoLog;

                //  Build a program.
                program.CreateInContext(gl);

                //  Attach the shaders.
                program.AttachShader(vertexShader);
                program.AttachShader(fragmentShader);
                program.Link();

                programs.Add(name, program);
                shaderErrors.Add(name, shaderError);
            }
        }

        public static void CompileShadersHotReload(OpenGL gl)
        {

            foreach (var name in shaderNames)
            {
                bool vertChanged = ResourceLoader.LoadTextFileIfChanged($"Shaders\\{name}.vert", out var vert);
                bool fragChanged = ResourceLoader.LoadTextFileIfChanged($"Shaders\\{name}.frag", out var frag);

                if (vertChanged || fragChanged)
                {
                    string shaderError = "";

                    ShaderProgram program = new ShaderProgram();

                    //  Create a vertex shader.
                    VertexShader vertexShader = new VertexShader();
                    vertexShader.CreateInContext(gl);
                    vertexShader.SetSource(vert);

                    //  Create a fragment shader.
                    FragmentShader fragmentShader = new FragmentShader();
                    fragmentShader.CreateInContext(gl);
                    fragmentShader.SetSource(frag);

                    //  Compile them both.
                    vertexShader.Compile();
                    fragmentShader.Compile();

                    if ((bool)!vertexShader.CompileStatus) shaderError += $"{name}.vert: " + vertexShader.InfoLog + "\n\n";
                    if ((bool)!fragmentShader.CompileStatus) shaderError += $"{name}.frag: " + fragmentShader.InfoLog;

                    //  Build a program.
                    program.CreateInContext(gl);

                    //  Attach the shaders.
                    program.AttachShader(vertexShader);
                    program.AttachShader(fragmentShader);
                    program.Link();

                    programs[name] = program;
                    shaderErrors[name] = shaderError;
                }
            }
        }

        public static string GetShaderErrors()
        {
            return shaderErrors.Values.Aggregate("", (acc, s) => acc + (s == "" ? "" : s + "\n\n"));
        }
    }
}
