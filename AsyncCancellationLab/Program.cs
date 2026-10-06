
using System;
using System.Threading;
using System.Threading.Tasks;
class Program
{
    static async Task Main(string[] args)
    {
        CancellationTokenSource cts = new CancellationTokenSource();

        cts.CancelAfter(TimeSpan.FromSeconds(5)); // Set a timeout of 2 seconds

        try
        {
            // Simulate a long-running operation
            await WaitForConnectionAsync(cts.Token);

            Console.WriteLine("접속 완료");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("접속 요청이 취소되었습니다.");
        }

        cts.Dispose();
    }

    static async Task WaitForConnectionAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            // Simulate checking for a connection
            Console.WriteLine("접속 요청 중...");

            await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
    }
}