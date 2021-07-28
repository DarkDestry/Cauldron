using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;
using Cauldron.Core.Logging;

namespace Cauldron.IPC
{
    public static class ConnectionManager
    {
        private static TcpClient client;
        private static NetworkStream stream;

        private const int SEND_TIMEOUT = 2000;

        public static void Connect(string hostname = "localhost", int port = 2134)
        {
            if (client != null)
            {
                throw new Exception("TCP client already exist. There should not be 2 connections.");
            }

            client = new TcpClient();
            client.Connect(hostname, port);
            Logger.LogDebug("Cauldron","TCP connection established");
            client.SendTimeout = SEND_TIMEOUT;
            stream = client.GetStream();
        }

        public static async Task ConnectAsync(string hostname = "localhost", int port = 2134)
        {
            client = new TcpClient();
            client.SendTimeout = SEND_TIMEOUT;
            await client.ConnectAsync(hostname, port);
            stream = client.GetStream();
        }
        
        /// <summary>
        /// Sends the string through the active connection. Null byte message delimiter IS sent for you.
        /// </summary>
        /// <param name="data"> Raw byte array to be sent </param>
        public static void SendRawString(string s)
        {
            byte[] data = Encoding.ASCII.GetBytes(s);
            stream.Write(data, 0, data.Length);
            stream.Write(new byte[] {0x0}, 0, 0);
            Logger.LogDebug("Cauldron",$"Sent Raw String: {s}");
        }

        /// <summary>
        /// Sends the byte array through the active connection. NULL BYTE MESSAGE DELIMITER IS NOT SENT!
        /// </summary>
        /// <param name="data"> Raw byte array to be sent </param>
        public static void SendRawBytes(params byte[] data)
        {
            stream.Write(data, 0, data.Length);
            Logger.LogDebug("Cauldron", $"Sent Raw Bytes of length {data.Length}");
        }

        public static void SendCommand(Command command)
        {
            if (client == null || !client.Connected)
            {
                throw new Exception("TCP Client not connected. Cannot send command.");
            }
            
            byte[] data = command.toBytes();
            stream.Write(data, 0, data.Length);
            stream.Write(new byte[]{0x0}, 0, 1);
            Logger.LogDebug("Cauldron",$"Sent Command {command.toJsonString()}");
        }

        /// <summary>
        /// Retrieve the next command from the stream. BLOCKING!
        /// </summary>
        /// <returns></returns>
        public static IncomingCommand GetCommand()
        {
            if (client == null || !client.Connected)
            {
                throw new Exception("TCP Client not connected. Cannot get command.");
            }
            
            StringBuilder sb = new StringBuilder();
            byte[] data = new byte[2048];
            while (true)
            {
                int bytesRead = stream.Read(data, 0, data.Length);
                Logger.LogDebug("Cauldron", $"Received TCP packet of size {bytesRead}, with data {Encoding.ASCII.GetString(data, 0, bytesRead)}");
                if (bytesRead == 1 && data[0] == 0)
                    break;
                sb.Append(Encoding.ASCII.GetString(data,0,bytesRead));
            }

            string incomingCommandJson = sb.ToString();

            return JsonSerializer.Deserialize<IncomingCommand>(incomingCommandJson);
        }

        public static bool HasIncomingBytes()
        {
            return stream.DataAvailable;
        }

        public static void Close()
        {
            if (client == null || !client.Connected)
            {
                throw new Exception("TCP client does not exist or has already been closed.");
            }

            client.Close();
        }
    }
}
