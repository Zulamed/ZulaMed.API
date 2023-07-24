using Vogen;

namespace ZulaMed.API;

file record Response(int StatusCode, string Message, string Error);

public class VogenValidationMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValueObjectValidationException e)
        {
            context.Response.StatusCode = 400;
            await context.Response.WriteAsJsonAsync(
                new Response(400, "Validation failure. See errors", e.Message));
        }
    }
}