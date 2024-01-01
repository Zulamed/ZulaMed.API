using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using ZulaMed.API.Data;
using ZulaMed.API.Domain.User;

namespace ZulaMed.API;

public class CleanUpUnverifiedUserBackgroundService : BackgroundService
{
    private readonly FirebaseAuth _firebaseAuth;
    private readonly IDbContextFactory<ZulaMedDbContext> _dbContextFactory;
    private readonly ILogger<CleanUpUnverifiedUserBackgroundService> _logger;

    public CleanUpUnverifiedUserBackgroundService(FirebaseAuth firebaseAuth,IDbContextFactory<ZulaMedDbContext> dbContextFactory,
        ILogger<CleanUpUnverifiedUserBackgroundService> logger)
    {
        _firebaseAuth = firebaseAuth;
        _dbContextFactory = dbContextFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Cleanup service is running");
        await CleanUp(stoppingToken);
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(60));

        try
        {
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await CleanUp(stoppingToken);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Cleanup service is stopping");
        }
    }

    private async Task CleanUp(CancellationToken token)
    {
        _logger.LogInformation("Cleanup service is cleaning up unverified users");
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(token);
        var query =
            dbContext.Set<User>().Where(x => x.IsVerified == IsVerified.From(false) && (DateTime)x.RegistrationTime < DateTime.UtcNow.AddDays(-1));
        var emails = await query.Select(x => x.Email).ToListAsync(cancellationToken: token);
        if (emails.Count == 0)
            return;
        foreach (var email in emails)
        {
            try
            {
                var user = await _firebaseAuth.GetUserByEmailAsync(email.Value, token);
                await Task.Delay(200, token);
                await _firebaseAuth.DeleteUserAsync(user.Uid, token);
            }
            catch (FirebaseAuthException e)
            {
                _logger.LogError(e, "Failed to delete user with email {Email}", email);
            }
        }
        await query.ExecuteDeleteAsync(cancellationToken: token);
        _logger.LogInformation("Cleanup service has cleaned up {Count} unverified users", emails.Count);
    }
}

public static class ConfigureBackgroundServices
{
    public static IServiceCollection AddCleanUpService(this IServiceCollection services)
    {
        return services.AddHostedService<CleanUpUnverifiedUserBackgroundService>();
    }
}
