using System.Diagnostics;
using FFMpegCore;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
});

app.Run();