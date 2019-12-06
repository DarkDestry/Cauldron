using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    public static class ResourceLoader
    {
        public static string LoadEmbeddedTextFile(string relativePath)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            var pathToDots = relativePath.Replace("\\", ".");
            var location = string.Format("{0}.{1}", executingAssembly.GetName().Name, pathToDots);

            using (var stream = executingAssembly.GetManifestResourceStream(location))
            {
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        public static string LoadTextFile(string relativePath)
        {
            string projectDirectory = Environment.CurrentDirectory;
//            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            return File.ReadAllText(projectDirectory + @"\" + relativePath);
        }

        private static Dictionary<string, long> filesize = new Dictionary<string, long>();
        public static bool LoadTextFileIfChanged(string relativePath, out string content)
        {
            string projectDirectory = Environment.CurrentDirectory;
//            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            long size = new FileInfo(projectDirectory + @"\" + relativePath).Length;

            if (!filesize.ContainsKey(relativePath)) filesize[relativePath] = 0;

            content = LoadTextFile(relativePath);

            if (size != filesize[relativePath])
            {
                filesize[relativePath] = size;
                return true;
            }
            return false;
        }
    }
}
