global using static Microsoft.AspNetCore.Http.TypedResults;
using Amazon.S3;
using OneOf;
using OneOf.Types;
using ZulaMed.VideoConversion;
using ZulaMed.VideoConversion.Endpoints.Transcode.Commands;
using ZulaMed.VideoConversion.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddValidators();
builder.Services.AddCqrs();
builder.Services
    .DecorateCommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>,
        TranscodeVideoCommandHandlerDecorator>();

builder.Services.AddOpenApiDocument();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddOptions<S3BucketOptions>()
    .BindConfiguration("S3BucketOptions")
    .ValidateDataAnnotations();


var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
    app.Map("/", () => Redirect("/swagger"));
}

app.MapEndpoints();


app.Run();