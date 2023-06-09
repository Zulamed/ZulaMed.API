using Amazon.S3;
using Amazon.SQS;
using ZulaMed.VideoConversion;
using ZulaMed.VideoConversion.Features.Transcode;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddOptions<S3BucketOptions>()
    .BindConfiguration("S3BucketOptions");
builder.Services.AddOptions<SqsQueueOptions>()
    .BindConfiguration("SQSQueueOptions");

builder.Services.AddMediator(options =>
{
    options.ServiceLifetime = ServiceLifetime.Scoped;
});


builder.Services.AddHostedService<Worker>();


var app = builder.Build();

app.Run();



// var builder = WebApplication.CreateBuilder(args);
//
// builder.Services.AddEndpoints();
// builder.Services.AddValidators();
// builder.Services.AddCqrs();
// builder.Services
//     .DecorateCommandHandler<TranscodeVideoCommand, OneOf<Success<string>, Error>,
//         TranscodeVideoCommandHandlerDecorator>();
//
// builder.Services.AddOpenApiDocument();
// builder.Services.AddEndpointsApiExplorer();
//
//
// builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
// builder.Services.AddOptions<S3BucketOptions>()
//     .BindConfiguration("S3BucketOptions")
//     .ValidateDataAnnotations();
//
//
// var app = builder.Build();
//
// if (!app.Environment.IsProduction())
// {
//     app.UseOpenApi();
//     app.UseSwaggerUi3();
//     app.Map("/", () => Redirect("/swagger"));
// }
//
// app.MapEndpoints();
//
//
// app.Run();
