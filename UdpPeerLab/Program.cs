using System.Net;
using System.Net.Sockets;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: UdpPeerLab <localPort> <remotePort> <peerName>");
            return;
        }

        if (!int.TryParse(args[0], out int localPort) ||
            !int.TryParse(args[1], out int remotePort) ||
            string.IsNullOrWhiteSpace(args[2]))
        {
            Console.WriteLine("Usage: UdpPeerLab <localPort> <remotePort> <peerName>");
            return;
        }

        string peerName = args[2];

        using UdpClient udpClient = new UdpClient(new IPEndPoint(IPAddress.Loopback, localPort));
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Loopback, remotePort);

        Console.WriteLine($"{peerName}: 내 포트 {localPort}, 상대 포트 {remotePort}");
        Console.WriteLine("메시지를 입력하세요. /exit으로 종료합니다.");

        Task receiveTask = ReceiveMessagesAsync(udpClient);
        Task sendTask = Task.Run(() => SendMessageAsync(udpClient, remoteEndPoint));

        await Task.WhenAll(receiveTask, sendTask);
    }

    private static async Task SendMessageAsync(UdpClient peer, IPEndPoint remoteEndPoint)
    {
        try
        {
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (input is null || input == "/exit")
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(input))
                {
                    byte[] data = Encoding.UTF8.GetBytes(input);
                    await peer.SendAsync(data, data.Length, remoteEndPoint);
                    Console.WriteLine($"보낸 메시지: {input}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"메시지 전송 중 오류 발생: {ex.Message}");
        }
        finally
        {
            peer.Close();
        }
    }

    private static async Task ReceiveMessagesAsync(UdpClient peer)
    {
        try
        {
            while (true)
            {
                UdpReceiveResult result = await peer.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                Console.WriteLine($"받은 메시지: {receivedMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"메시지 수신 중 오류 발생: {ex.Message}");
        }
    }
}