using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;

namespace ZulaMed.API.Endpoints.MuxWebhook;

public interface IMuxWebhookValidator
{
    public bool Validate(string signature, string timestamp, string body);
}

public class MuxWebhookValidator : IMuxWebhookValidator
{
    private readonly IOptions<MuxSettings> _muxSettings;

    public MuxWebhookValidator(IOptions<MuxSettings> muxSettings)
    {
        _muxSettings = muxSettings;
    }

    public bool Validate(string signature, string timestamp, string body)
    {
        var secret = _muxSettings.Value.SigningKey;
        var payload = $"{timestamp}.{body}";
        using var hmac = new HMACSHA256();
        hmac.Key = Encoding.UTF8.GetBytes(secret);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var hex = BitConverter.ToString(hash).Replace("-", "").ToLower();
        return hex == signature;
    }
}