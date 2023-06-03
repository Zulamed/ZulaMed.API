global using static Microsoft.AspNetCore.Http.TypedResults;
using Amazon.S3;
using ZulaMed.VideoConversion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpoints();
builder.Services.AddValidators();

builder.Services.AddOpenApiDocument();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();

var app = builder.Build();

if (!app.Environment.IsProduction())
{
    app.UseOpenApi();
    app.UseSwaggerUi3();
    app.Map("/", () => Redirect("/swagger"));
}

app.MapEndpoints();


app.Run();