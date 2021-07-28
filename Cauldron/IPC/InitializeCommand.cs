using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Cauldron.CustomControls;

namespace Cauldron.IPC
{
    public class InitializeCommand : Command
    {
        [JsonPropertyName("args")] 
        public Args args { get; }
        
        [JsonConstructor]
        public InitializeCommand(IntPtr hwnd) : base(CommandType.Initialize)
        {
            args = new Args(hwnd);
        }

        public class Args
        {
            [JsonPropertyName("hwnd")] 
            [JsonConverter(typeof (IntPtrJsonConverter))]
            public IntPtr hwnd { get; }

            [JsonConstructor]
            public Args(IntPtr hwnd)
            {
                this.hwnd = hwnd;
            }
        }
    }

    public class IncomingInitializeCommand : IncomingCommand
    {
        [JsonPropertyName("args")] 
        public Args args { get; set; }

        public override void ExecuteCommand()
        {
            Control_Ether_HwndHost.rendererControl.HwndRenderer = args.childHwnd;
            Control_Ether_HwndHost.Resize();
        }

        public class Args
        {
            [JsonPropertyName("childhwnd")] 
            [JsonConverter(typeof (IntPtrJsonConverter))]
            public IntPtr childHwnd { get; set; }
        }
    }

    public class IntPtrJsonConverter : JsonConverter<IntPtr>
    {
        public override IntPtr Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return (IntPtr) reader.GetUInt64();
        }

        public override void Write(Utf8JsonWriter writer, IntPtr value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((ulong) value);
        }
    }
}
