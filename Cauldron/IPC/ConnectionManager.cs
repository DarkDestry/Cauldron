using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Media.TextFormatting;

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

            client = new TcpClient(hostname, port);
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

        public static void SendCommand(Command command)
        {
            if (client == null || !client.Connected)
            {
                throw new Exception("TCP Client not connected. Cannot send command.");
            }

            byte[] data = command.toBytes();
            stream.Write(data, 0, data.Length);
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
                if (bytesRead == 1 && data[0] == 0x0)
                    break;
                sb.Append(Encoding.ASCII.GetString(data,0,bytesRead));
            }

            string incomingCommandJson = sb.ToString();

            return JsonSerializer.Deserialize<IncomingCommand>(incomingCommandJson);
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
