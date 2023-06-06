using System.Text.RegularExpressions;
using FluentValidation;

namespace ZulaMed.VideoConversion.Endpoints.Transcode;


public struct Resolution
{
    public required int Width { get; init; }
    public required int Height { get; init; }
}

public class RequestBody
{
    public required string S3Path { get; set; }
}

public partial class Validator : AbstractValidator<RequestBody>
{
    [GeneratedRegex(@"videos\/[a-fA-F0-9]{8}-(?:[a-fA-F0-9]{4}-){3}[a-fA-F0-9]{12}", 
            RegexOptions.IgnoreCase | RegexOptions.Compiled, "en-US")]
        private static partial Regex FormatValidationRegex();
    
    
    public Validator()
    {
        RuleFor(x => x.S3Path)
            .Must(x => FormatValidationRegex().IsMatch(x))
            .WithMessage("URL must be in the format '/videos/GUID'");
    } 
}


