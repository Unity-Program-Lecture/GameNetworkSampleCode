using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("접속 요청 시작");

        // Call the asynchronous method
        string result = await ConnectAsync();

        Console.WriteLine(result);
    }

    static async Task<string> ConnectAsync()
    {
        // Simulate a long-running operation
        await Task.Delay(TimeSpan.FromSeconds(1));

        Console.WriteLine("1초 대기");

        await Task.Delay(TimeSpan.FromSeconds(2));

        Console.WriteLine("2초 대기");

        await Task.Delay(TimeSpan.FromSeconds(1));

        return "접속 완료";
    }
}
