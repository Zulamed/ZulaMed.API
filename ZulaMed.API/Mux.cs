using System.Net.Http.Headers;
using System.Text;
using Refit;
using ZulaMed.API.Endpoints.VideoRestApi;

namespace ZulaMed.API;


public class MuxSettings
{
    public required string SecretKey { get; set; } 
    
    public required string Id { get; set; }
}


public static class Mux
{
    public static IServiceCollection AddMux(this IServiceCollection services, string muxSecretKey, string muxTokenId)
    {
        services.AddRefitClient<IMuxClientApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.mux.com");
                var base64String = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{muxTokenId}:{muxSecretKey}"));
                client.DefaultRequestHeaders.Authorization = new 
                    AuthenticationHeaderValue("Basic", base64String);
            });
        
        services.AddOptions<MuxSettings>()
            .BindConfiguration("MuxSettings")
            .ValidateDataAnnotations();

        return services;
    }
}