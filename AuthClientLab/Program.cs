using System.Net.Http;
using System.Text;
using System.Text.Json;

public class Program
{
    private const string ServerUrl = "https://localhost:5001"; // AuthServerLab의 URL

    private static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        Console.WriteLine("아이디:");
        string id = Console.ReadLine();
        Console.WriteLine("비밀번호:");
        string password = Console.ReadLine();

        LoginRequest loginRequest = new LoginRequest
        {
            Id = id,
            Password = password
        };

        string loginRequestJson = JsonSerializer.Serialize(loginRequest);

        using (HttpClient client = new HttpClient())
        {
            using (StringContent content = new StringContent(loginRequestJson, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage response = await client.PostAsync(ServerUrl + "/login", content);

                if (response.IsSuccessStatusCode)
                {
                    string responseJson = await response.Content.ReadAsStringAsync();

                    LoginResponse loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseJson);

                    Console.WriteLine($"로그인 성공! PlayerId: {loginResponse.PlayerId}, AccessToken: {loginResponse.AccessToken}");
                }
                else
                {
                    Console.WriteLine("로그인 실패!");
                }
            }
        }
    }

    public class LoginRequest
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public bool IsSuccess { get; set; }

        public string AccessToken { get; set; }
        public int PlayerId { get; set; }
    }
}
