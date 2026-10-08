
using System.Net;
using System.Net.Sockets;

namespace JsonPacket.Server
{
    internal class Program
    {
        private static readonly List<Session> Clients = new List<Session>();
        private static readonly object _clientLock = new object();

        public static async Task Main()
        {
            TcpListener listener = new TcpListener(IPAddress.Loopback, 7777);
            listener.Start();
            Console.WriteLine("TCP Server is running on port 7777...");

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();

                _ = HandleClientAsync(client);
            }
        }

        private static async Task HandleClientAsync(TcpClient client)
        {
            using (client)
            {
                using NetworkStream stream = client.GetStream();

                while (true)
                {
                    RecievedPacket? recieved = await PacketTransfer.ReceivePacketAsync(stream);
                    if (recieved is null)
                    {
                        break;
                    }

                    switch (recieved.Type)
                    {
                        case PacketType.Chat:
                            ChatPacket? chatPacket = PacketJsonConverter.Deserialize<ChatPacket>(recieved.Json);
                            await PacketTransfer.SendPacketAsync(new ChatPacket { Message = $"[Server] {chatPacket.Message}" }, stream);
                            break;

                        case PacketType.System:
                            SystemPacket? systemPacket = PacketJsonConverter.Deserialize<SystemPacket>(recieved.Json);
                            await PacketTransfer.SendPacketAsync(new SystemPacket { Command = $"[Server] {systemPacket.Command}" }, stream);
                            break;

                        default:
                            break;
                    }

                }
            }
        }

        private static async Task BroadcastMesageAsync(string message)
        {
            Session[]? sessions = null;

            lock (_clientLock)
            {
                sessions = Clients.ToArray();
            }

            foreach (Session client in sessions)
            {
                try
                {
                    await client.Writer.WriteLineAsync(message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending message to {client.Nickname}: {ex.Message}");
                }
            }
        }

        private class Session
        {
            public string Nickname { get; set; }
            public StreamWriter Writer { get; }

            public Session(string nickname, StreamWriter writer)
            {
                Nickname = nickname;
                Writer = writer;
            }
        }
    }
}
