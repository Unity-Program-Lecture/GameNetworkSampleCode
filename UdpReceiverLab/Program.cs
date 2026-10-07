using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using UdpClient receiver = new UdpClient(7778);
        Console.WriteLine("Listening for UDP packets on port 7778...");

        UdpReceiveResult result = await receiver.ReceiveAsync();

        string message = Encoding.UTF8.GetString(result.Buffer);

        Console.WriteLine($"Received message: {message}");
    }
}
