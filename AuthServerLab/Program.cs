using System;
using System.Collections.Concurrent;

public class Program
{
    private static readonly ConcurrentDictionary<string, int> IssuedTokens = new ConcurrentDictionary<string, int>();

    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        WebApplication app = builder.Build();

        app.MapPost("/login", Login);
        app.MapPost("/validate", Validate);

        app.Run("https://localhost:5001");
    }

    private static IResult Login(LoginRequest request)
    {
        if (request.Id != "player01" || request.Password != "1234")
        {
            return Results.Unauthorized();
        }

        string accessToken = Guid.NewGuid().ToString("N");
        IssuedTokens[accessToken] = 101; // PlayerId를 101로 설정

        LoginResponse response = new LoginResponse
        {
            IsSuccess = true,
            AccessToken = accessToken,
            PlayerId = 101
        };

        return Results.Ok(response);
    }

    private static IResult Validate(TokenRequest request)
    {
        if (!IssuedTokens.TryGetValue(request.AccessToken, out int playerId))
        {
            return Results.Unauthorized();
        }

        ValidateResponse response = new ValidateResponse
        {
            PlayerId = playerId
        };

        return Results.Ok(response);
    }

    // LoginRequest는 클라이언트가 보내는 로그인 JSON 구조입니다.
    public class LoginRequest
    {
        public string Id { get; set; }
        public string Password { get; set; }
    }

    // LoginResponse는 서버가 돌려주는 로그인 JSON 구조입니다.
    public class LoginResponse
    {
        public bool IsSuccess { get; set; }

        public string AccessToken { get; set; }
        public int PlayerId { get; set; }
    }

    // TokenRequest는 토큰 검증 요청의 JSON 구조입니다.
    public class TokenRequest
    {
        public string AccessToken { get; set; }
    }

    // ValidateResponse는 토큰 검증 성공 뒤 돌려주는 JSON 구조입니다.
    public class ValidateResponse
    {
        public int PlayerId { get; set; }
    }
}
