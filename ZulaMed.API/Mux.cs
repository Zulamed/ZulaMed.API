using Mux.Csharp.Sdk.Api;
using Mux.Csharp.Sdk.Client;
using ZulaMed.API.Endpoints.MuxWebhook;

namespace ZulaMed.API;


public class MuxSettings
{
    public required string SecretKey { get; set; } 
    
    public required string Id { get; set; }
    
    public required string SigningKey { get; set; }
}


public static class Mux
{
    public static IServiceCollection AddMux(this IServiceCollection services, string muxSecretKey, string muxTokenId)
    {
        var configuration = new Configuration
        {
            BasePath = "https://api.mux.com",
            Username = muxTokenId,
            Password = muxSecretKey
        };

        services.AddSingleton(new DirectUploadsApi(configuration));
        services.AddSingleton(new LiveStreamsApi(configuration));

        services.AddOptions<MuxSettings>()
            .BindConfiguration("MuxSettings")
            .ValidateDataAnnotations();

        services.AddSingleton<IMuxWebhookValidator, MuxWebhookValidator>();

        return services;
    }
}