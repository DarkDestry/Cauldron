using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Cauldron.IPC
{
    public class ResizeCommand : Command
    {
        [JsonPropertyName("args")] public Args args { get; }

        [JsonConstructor]
        public ResizeCommand(uint width, uint height) : base(CommandType.Resize)
        {
            args = new Args(width, height);
        }

        public class Args
        {
            [JsonPropertyName("width")] public uint Width { get; }

            [JsonPropertyName("height")] public uint Height { get; }

            [JsonConstructor]
            public Args(uint width, uint height)
            {
                this.Width = width;
                this.Height = height;
            }
        }
    }
}
