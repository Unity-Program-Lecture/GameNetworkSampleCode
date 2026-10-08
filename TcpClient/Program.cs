using System.Net;
using System.Net.Sockets;

internal class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Nickname :");
        string nickname = Console.ReadLine() ?? "Anonymous";

        using TcpClient client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, 7777);

        using NetworkStream stream = client.GetStream();
        using StreamReader reader = new StreamReader(stream);
        using StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        await writer.WriteLineAsync(nickname);

        Console.WriteLine("Start input message.");

        Task recieveTask = RecieveMessagesAsync(reader);
        Task sendTask = Task.Run(() => SendMessagesAsync(client, writer));

        await Task.WhenAll(recieveTask, sendTask);
    }

    private static async Task RecieveMessagesAsync(StreamReader reader)
    {
        try
        {
            while (true)
            {
                string? message = await reader.ReadLineAsync();
                if (message is null)
                {
                    break;
                }

                string[] splitMessage = message.Split('|');
                if (splitMessage.Length != 2)
                {
                    continue;
                }

                switch (splitMessage[0])
                {
                    case "CHAT":
                        Console.WriteLine(splitMessage[1]);
                        break;

                    case "SYSTEM":
                        Console.WriteLine($"[SYSTEM] {splitMessage[1]}");
                        break;
                }


            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error receiving messages: {ex.Message}");
        }
    }

    private static async Task SendMessagesAsync(TcpClient client, StreamWriter writer)
    {
        try
        {
            while (true)
            {
                string? message = Console.ReadLine();
                if (message is null || message == "/exit")
                {
                    break;
                }

                if (!string.IsNullOrWhiteSpace(message))
                {
                    await writer.WriteLineAsync($"CHAT|{message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending messages: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }
}
