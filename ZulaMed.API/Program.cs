using Amazon;
using Amazon.S3;
using Amazon.SQS;
using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using ZulaMed.API;
using ZulaMed.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = int.MaxValue;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.AddCors();


builder.Services.AddFastEndpoints(options => { options.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All; });

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.DocumentName = "Initial Release";
        s.Title = "Web API";
        s.Version = "v0.0";
        s.SchemaType = NJsonSchema.SchemaType.OpenApi3;
    };
    o.SerializerSettings = x => x.PropertyNamingPolicy = null;
    o.TagCase = TagCase.TitleCase;
    o.RemoveEmptyRequestSchema = false;
});

builder.Services.AddOptions<S3BucketOptions>()
    .BindConfiguration("S3BucketOptions")
    .ValidateDataAnnotations();

builder.Services.AddOptions<SqsQueueOptions>()
    .BindConfiguration("SQSQueueOptions")
    .ValidateDataAnnotations();

AWSConfigsS3.EnableUnicodeEncodingForObjectMetadata = true;
builder.Services.AddSingleton<IAmazonS3, AmazonS3Client>();
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();

builder.Services.AddMediator(x => { x.ServiceLifetime = ServiceLifetime.Scoped; });

var connectionString = string.IsNullOrEmpty(builder.Configuration["DATABASE_CONNECTION_STRING"]) 
    ? builder.Configuration["Database:ConnectionString"] 
    : builder.Configuration["DATABASE_CONNECTION_STRING"];

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
var dataSource = dataSourceBuilder.Build();


builder.Services.AddDbContext<ZulaMedDbContext>(options =>
{
    options.UseNpgsql(dataSource);
});

var app = builder.Build();

app.UseCors(c =>
{
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
});

app.UseAuthorization();
app.UseFastEndpoints();

if (!app.Environment.IsProduction())
{
    app.Map("/", () => Results.Redirect("/swagger"));
    app.UseSwaggerGen(uiConfig: settings => settings.ConfigureDefaults());
}

app.Run();