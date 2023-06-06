using FluentValidation;
using ZulaMed.VideoConversion.Endpoints;
using ZulaMed.VideoConversion.Infrastructure;

namespace ZulaMed.VideoConversion;

// the scanning can be replaced with source generators or manual registration. For the ease of use, scanning is used 
public static class Extensions
{
    
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

    public static IServiceCollection AddCqrs(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c => c.AssignableTo(typeof(IQueryHandler<,>)))
            .AsImplementedInterfaces()
            .WithTransientLifetime()
        );

        services.Scan(scan => scan
            .FromAssemblyOf<Program>()
            .AddClasses(c =>
            {
                c.AssignableTo(typeof(ICommandHandler<,>));
                c.Where(x => !x.Name.Contains("Decorator"));
            })
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