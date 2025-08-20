using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WordCountAPI.HealthChecks;

public class DegradedHealthCheck: IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Degraded("Degraded"));
    }
}