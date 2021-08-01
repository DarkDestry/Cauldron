using System;
using System.Diagnostics.SymbolStore;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cauldron.IPC
{
    public abstract class Command
    {
        [JsonPropertyName("command")]
        public CommandType IpcCommand { get; }

        [JsonConstructor]
        public Command(CommandType command)
        {
            IpcCommand = command;
        }

        public virtual byte[] toBytes()
        {
            return System.Text.Encoding.ASCII.GetBytes(toJsonString());
        }

        public virtual string toJsonString()
        {
            return JsonSerializer.Serialize<object>(this);
        }
    }

    [JsonConverter(typeof(IncomingCommandJsonConverter))]
    public abstract class IncomingCommand
    {
        [JsonPropertyName("command")]
        public CommandType IpcCommand { get; }

        public abstract void ExecuteCommand();
    }
    
    [JsonConverter(typeof(CommandTypeJsonConverter))]
    public enum CommandType
    {
        Initialize,
        Shutdown,
        Resize,
        LoadWorld,
        SaveWorld,
        Unknown
    }

    public class CommandTypeJsonConverter : JsonConverter<CommandType>
    {
        public override CommandType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string command = reader.GetString();
            switch(command)
            {
                case "initialize" : return CommandType.Initialize;
                case "shutdown" : return CommandType.Shutdown;
                case "resize" : return CommandType.Resize;
                case "loadworld" : return CommandType.LoadWorld;
                case "saveworld" : return CommandType.SaveWorld;
                default: return CommandType.Unknown;
            }
        }

        public override void Write(Utf8JsonWriter writer, CommandType value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString().ToLower());
        }
    }

    public class IncomingCommandJsonConverter : JsonConverter<IncomingCommand>
    {
        public override bool CanConvert(Type type)
        {
            return typeof(IncomingCommand).IsAssignableFrom(type);
        }

        public override IncomingCommand Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (JsonDocument.TryParseValue(ref reader, out JsonDocument document))
            {
                if (document.RootElement.TryGetProperty("command", out JsonElement commandType))
                {
                    CommandType incomingCommandType = JsonSerializer.Deserialize<CommandType>(commandType.GetRawText());
                    string rootElement = document.RootElement.GetRawText();

                    //TODO: Potentially change this to use reflection instead
                    switch (incomingCommandType)
                    {
                        case CommandType.Initialize:
                            return JsonSerializer.Deserialize<IncomingInitializeCommand>(rootElement);
                        case CommandType.Shutdown:
                            break;
                        case CommandType.Resize:
                            break;
                        case CommandType.LoadWorld:
                            break;
                        case CommandType.SaveWorld:
                            break;
                        default:
                            throw new JsonException("Unknown Command Type found in incoming command");
                    }
                }

                throw new JsonException("Failed to extract command type from incoming command. Might it be missing?");
            }

            throw new JsonException("Incoming Command failed to Parse");
        }

        public override void Write(Utf8JsonWriter writer, IncomingCommand value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}