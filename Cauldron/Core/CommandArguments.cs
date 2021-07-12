using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cauldron.Core
{
    static class CommandArguments
    {
        private static Dictionary<string, string> args = new Dictionary<string, string>();

        public static void SetArgument(string argument)
        {
            SetArgument(argument, "");
        }

        public static void SetArgument(string argument, string value)
        {
            if (args.ContainsKey(argument))
            {
                throw new ArgumentException("Duplicate argument: " + argument);
            }
            args.Add(argument, value);
        }

        public static bool TryGetArgument(string argument, out string value)
        {
            if (!args.ContainsKey(argument))
            {
                value = null;
                return false;
            }

            value = args[argument];
            return true;
        }

        public static bool TryGetArgument(string argument)
        {
            if (args.ContainsKey(argument)) return true;
            return false;
        }
    }
}
