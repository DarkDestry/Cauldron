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
            while(true)
            try
            {
                string workingDirectory = Environment.CurrentDirectory;
                string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
                return File.ReadAllText(workingDirectory + @"\" + relativePath);
            }
            catch (IOException e)
            {
                Console.WriteLine(e);
            }
        }

        private static Dictionary<string, long> filehash = new Dictionary<string, long>();
        public static bool LoadTextFileIfChanged(string relativePath, out string content)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            content = LoadTextFile(relativePath);
            int hash = content.GetHashCode();

            if (!filehash.ContainsKey(relativePath)) filehash[relativePath] = 0;

            if (hash != filehash[relativePath])
            {
                filehash[relativePath] = hash;
                return true;
            }
            return false;
        }
    }
}
