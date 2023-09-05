using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ZulaMed.API.Health;

public class StubHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy());
    }
}