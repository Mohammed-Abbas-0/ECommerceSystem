using Microsoft.Extensions.Logging;
using Polly;
using Polly.Timeout;

namespace Orders.Infrastructure.Resilience;

public static class ResiliencePolicies
{
    public static IAsyncPolicy GetRetryPolicy(ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    logger.LogWarning(
                        "⚠️ Retry {RetryCount} after {Delay}s - Error: {Error}",
                        retryCount,
                        timeSpan.TotalSeconds,
                        exception.Message);
                });
    }

    public static IAsyncPolicy GetCircuitBreakerPolicy(ILogger logger)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 5,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    logger.LogError(
                        "🔴 Circuit Breaker OPEN for {Duration}s - Error: {Error}",
                        duration.TotalSeconds,
                        exception.Message);
                },
                onReset: () =>
                {
                    logger.LogInformation(
                        "🟢 Circuit Breaker CLOSED - Service recovered");
                },
                onHalfOpen: () =>
                {
                    logger.LogInformation(
                        "🟡 Circuit Breaker HALF-OPEN - Testing service");
                });
    }

    public static IAsyncPolicy GetTimeoutPolicy()
    {
        return Policy.TimeoutAsync(
            seconds: 3,
            timeoutStrategy: TimeoutStrategy.Optimistic);
    }

    public static IAsyncPolicy GetCombinedPolicy(ILogger logger)
    {
        return Policy.WrapAsync(
            GetRetryPolicy(logger),
            GetCircuitBreakerPolicy(logger),
            GetTimeoutPolicy());
    }
}