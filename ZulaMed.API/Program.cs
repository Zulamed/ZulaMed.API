using FastEndpoints;
using FastEndpoints.Swagger;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Refit;
using ZulaMed.API;
using ZulaMed.API.Endpoints.ChatAI.OpenAISerivce;
using ZulaMed.API.Endpoints.UserRestApi.Verify;
using ZulaMed.API.Health;

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

builder.Configuration["MuxSettings:Secret"] = string.IsNullOrEmpty(builder.Configuration["MUX_SECRET"])
    ? builder.Configuration["MuxSettings:Secret"]
    : builder.Configuration["MUX_SECRET"];

builder.Configuration["MuxSettings:SigningKey"] = string.IsNullOrEmpty(builder.Configuration["MUX_SIGNING_KEY"])
    ? builder.Configuration["MuxSettings:SigningKey"]
    : builder.Configuration["MUX_SIGNING_KEY"];

builder.Configuration["Firebase:ApiKey"] = string.IsNullOrEmpty(builder.Configuration["FIREBASE_API_KEY"])
    ? builder.Configuration["Firebase:ApiKey"]
    : builder.Configuration["FIREBASE_API_KEY"];


builder.Services.AddMux(builder.Configuration["MuxSettings:Secret"]!,
    builder.Configuration["MuxSettings:Id"]!);


builder.Services.AddFastEndpoints(options =>
{
    options.SourceGeneratorDiscoveredTypes.AddRange(DiscoveredTypes.All);
});

builder.Services.AddAuthorization();

builder.Services.SwaggerDocument(o =>
{
    o.DocumentSettings = s =>
    {
        s.Title = "ZulaMed API";
        s.Version = "v1";
    };
});

builder.Services.AddSingleton<IOpenAIService, OpenAIService>();

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

builder.Services.AddCleanUpService();

builder.Services.AddRefitClient<IFirebaseEmailVerifier>()
    .ConfigureHttpClient(c => { c.BaseAddress = new Uri("https://identitytoolkit.googleapis.com/v1"); });

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