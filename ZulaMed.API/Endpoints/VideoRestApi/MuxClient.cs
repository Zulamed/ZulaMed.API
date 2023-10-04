using System.Text.Json.Serialization;
using Refit;

namespace ZulaMed.API.Endpoints.VideoRestApi;

public class MuxRequestBody
{
    public static MuxRequestBody Default => new()
    {
        CorsOrigin = "*",
        NewAssetSettings = new NewAssetSettings
        {
            PlaybackPolicy = new[] { "public" },
            MaxResolutionTier = "2160p"
        }
    };

    [JsonPropertyName("cors_origin")] public required string CorsOrigin { get; set; }


    [JsonPropertyName("new_asset_settings")]
    public required NewAssetSettings NewAssetSettings { get; set; }
}

public class MuxResponse 
{
    [JsonPropertyName("data")]
    public required MuxResponseData Data { get; set; }
}

public class MuxResponseData
{
    [JsonPropertyName("url")]
    public required string Url { get; set; }
    
    [JsonPropertyName("timeout")]
    public required int Timeout { get; set; }
    
    [JsonPropertyName("status")]
    public required string Status { get; set; }
    
    [JsonPropertyName("new_asset_settings")]
    public required ResponseNewAssetSettings NewAssetSettings { get; set; }
    
    [JsonPropertyName("id")]
    public required string Id { get; set; }
    
    [JsonPropertyName("cors_origin")]
    public required string CorsOrigin { get; set; }
}

public class ResponseNewAssetSettings
{
    [JsonPropertyName("playback_policies")]
    public required string[] PlaybackPolicies { get; set; }
    
    [JsonPropertyName("mp4_support")]
    public string? Mp4Support { get; set; }
}




public class NewAssetSettings
{
    [JsonPropertyName("playback_policy")] public required string[] PlaybackPolicy { get; set; }

    // This needs to be rewritten to use an enum but json serialization is being a pain, bcs
    // enum values are written different than the accepted values
    // Solutions to this: Use a enum library(SmartEnum, Intellenum) or use a custom json converter
    // Acceptable values: "1080p", "1440p", "2160p"
    public required string MaxResolutionTier { get; set; }
}

public interface IMuxClientApi
{
    [Post("/video/v1/uploads")]
    Task<IApiResponse<MuxResponse>> GetUploadUrl([Body] MuxRequestBody body);
}