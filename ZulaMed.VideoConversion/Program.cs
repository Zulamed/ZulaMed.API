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
builder.Services.AddOptions<ResolutionOptions>()
    .BindConfiguration("VideoOptions");

builder.Services.AddMediator();


builder.Services.AddHostedService<Worker>();


var app = builder.Build();

app.Run();