using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ZulaMed.API;
using ZulaMed.API.Endpoints.VideoRestApi;
using ZulaMed.API.Endpoints.VideoRestApi.Upload;
using ZulaMed.API.Health;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options => { options.Limits.MaxRequestBodySize = int.MaxValue; });

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = int.MaxValue;
});

builder.Services.AddCors();

builder.Services.AddMux(builder.Configuration["MuxSettings:Secret"]!,
    builder.Configuration["MuxSettings:Id"]!);



builder.Services.AddFastEndpoints(options => { options.SourceGeneratorDiscoveredTypes = DiscoveredTypes.All; });

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "Web API";
        s.Version = "v0.0";
        s.SchemaType = NJsonSchema.SchemaType.OpenApi3;
    };
    o.SerializerSettings = x => x.PropertyNamingPolicy = null;
    o.TagCase = TagCase.TitleCase;
    o.RemoveEmptyRequestSchema = false;
});

builder.Services.AddAmazon();

builder.Services.AddMediator(x => { x.ServiceLifetime = ServiceLifetime.Scoped; });

var connectionString = string.IsNullOrEmpty(builder.Configuration["DATABASE_CONNECTION_STRING"])
    ? builder.Configuration["Database:ConnectionString"]
    : builder.Configuration["DATABASE_CONNECTION_STRING"];

builder.Services.AddDatabase(connectionString);

var credential = await GoogleCredential.FromFileAsync("firebase-adminsdk-configuration.json",
    CancellationToken.None);

builder.Services.AddFirebase(credential);

builder.Services.AddTransient<VogenValidationMiddleware>();

builder.Services.AddHealthChecks()
    .AddCheck<StubHealthCheck>("Stub");


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["AuthSettings:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["AuthSettings:Authority"],
            ValidAudience = builder.Configuration["AuthSettings:Audience"],
        };
    });

var app = builder.Build();

app.UseCors(c =>
{
    c.AllowAnyMethod();
    c.AllowAnyOrigin();
    c.AllowAnyHeader();
    c.WithExposedHeaders("Location");
});

app.UseHealthChecks("/_health");

app.UseMiddleware<VogenValidationMiddleware>();
app.UseDefaultExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();

if (!app.Environment.IsProduction())
{
    app.Map("/", () => Results.Redirect("/swagger"));
    app.UseSwaggerGen();
}

app.Run();