using System.Net;
using System.Net.Sockets;

internal class Program
{
    private static List<Session> Clients = new List<Session>();

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
            using StreamReader reader = new StreamReader(stream);
            using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

            Session session = null;

            try
            {
                string nickname = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(nickname))
                {
                    return;
                }

                session = new Session(nickname, writer);
                Clients.Add(session);

                await BroadcastMesageAsync($"{nickname} has joined the chat.");

                while (true)
                {
                    string? message = await reader.ReadLineAsync();
                    if (message is null)
                    {
                        break;
                    }

                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        await BroadcastMesageAsync($"[{nickname}] {message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message}");
            }
            finally
            {
                if (session is not null)
                {
                    Clients.Remove(session);
                    Console.WriteLine($"{session.Nickname} has disconnected.");
                }
            }
        }
    }

    private static async Task BroadcastMesageAsync(string message)
    {
        foreach (Session client in Clients)
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
