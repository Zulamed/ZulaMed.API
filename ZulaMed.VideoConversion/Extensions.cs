using FluentValidation;
using ZulaMed.VideoConversion.Endpoints;

namespace ZulaMed.VideoConversion;

public static class Extensions
{
    // this could be source generated instead of using reflection
    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c => c.AssignableTo<IEndpoint>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        return services;
    }


    public static IServiceCollection AddValidators(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c => c.AssignableTo(typeof(IValidator<>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );
        
        
        return services;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        foreach (var endpoint in scope.ServiceProvider.GetServices<IEndpoint>())
        {
            endpoint.ConfigureRoute(app);
        }

        return app;
    }
}