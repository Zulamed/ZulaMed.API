using Refit;

namespace ZulaMed.API.Endpoints.Auth;

public record SignInRequest(string Email, string Password);

public record SignInResponse(string IdToken, string RefreshToken, string Email, string ExpiresIn);

public record SignInErrorResponse(Error Error);

public record Error(
    int Code,
    string Message,
    Errors[] Errors
);

public record Errors(
    string Message,
    string Domain,
    string Reason
);

public interface IFirebaseApiClient
{
    [Post("/v1/accounts:signInWithPassword?key={apiKey}")]
    Task<ApiResponse<SignInResponse>> SignInWithPasswordAsync(string apiKey, SignInRequest request);
}