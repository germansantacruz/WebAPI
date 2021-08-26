using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPI.HealthChecks
{
    public class MyRandomHealthCheck : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, 
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(HealthCheckResult.Healthy("good"));
            /*
            return (new Random().Next() % 2 == 0) ?
                        Task.FromResult(HealthCheckResult.Healthy("good")) :
                        Task.FromResult(HealthCheckResult.Degraded("not so good"));
            */
        }
    }
}
