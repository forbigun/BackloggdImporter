using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using BackloggdImporter.Helpers;
using Polly;
using Polly.Retry;

namespace BackloggdImporter.Factories;

internal static class PollyPipelineFactory
{
    public static ResiliencePipeline CreateResiliencePipeline()
    {
        return new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>(ex => ex.StatusCode == HttpStatusCode.Unauthorized),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromSeconds(15),
                BackoffType = DelayBackoffType.Exponential,
                OnRetry = args =>
                {
                    ConsolePrinter.WriteWarning($"Authentication failed - waiting {args.RetryDelay.TotalSeconds}s " +
                                                $"(attempt {args.AttemptNumber + 1})");
                    return ValueTask.CompletedTask;
                }
            })
            .AddRetry(new RetryStrategyOptions
            {
                ShouldHandle = new PredicateBuilder()
                    .Handle<HttpRequestException>(ex => ex.StatusCode == HttpStatusCode.TooManyRequests),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMinutes(1),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    ConsolePrinter.WriteWarning($"Rate limit - waiting {args.RetryDelay.TotalMinutes} min " +
                                                $"(attempt {args.AttemptNumber + 1})");
                    return ValueTask.CompletedTask;
                }
            })
            .AddTimeout(TimeSpan.FromSeconds(15))
            .Build();
    }
}