using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using UdpClient sender = new UdpClient();

        byte[] bytes = Encoding.UTF8.GetBytes("Hello, UDP!");

        await sender.SendAsync(bytes, bytes.Length, new IPEndPoint(IPAddress.Loopback, 7778));

        Console.WriteLine("Message sent to UDP receiver on port 7778.");
    }
}