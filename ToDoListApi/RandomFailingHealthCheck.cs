using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace ToDoListApi;

public class RandomFailingHealthCheck : IHealthCheck
{
	private static readonly Random _random = new Random();
	private	readonly ILogger _logger;

	public RandomFailingHealthCheck(ILogger<RandomFailingHealthCheck> logger)
	{
		_logger = logger;
	}

	public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
		CancellationToken cancellationToken = default)
	{
		var randomNumber = _random.Next(1, 101);

		if (randomNumber <= 30)
		{
            _logger.LogError($"Failed {DateTime.Now}");
			return Task.FromResult(HealthCheckResult.Unhealthy($"Failed"));
		}

		_logger.LogInformation($"Success {DateTime.Now}");
		return Task.FromResult(HealthCheckResult.Healthy($"Success"));
	}
}
