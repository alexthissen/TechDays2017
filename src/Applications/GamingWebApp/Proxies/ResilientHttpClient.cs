using Microsoft.Extensions.Logging;
using Polly;
using Polly.Wrap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace GamingWebApp.Proxies
{
    public class ResilientHttpClient : HttpClient
    {
        private PolicyWrap policyWrapper;
        private ILogger logger;

        public ResilientHttpClient(ILogger logger)
        {
            this.logger = logger;
            policyWrapper = Policy.WrapAsync(CreatePolicies());
        }

        public ResilientHttpClient(Policy[] policies, ILogger logger)
        {
            this.logger = logger;
            // Add Policies to be applied
            policyWrapper = Policy.WrapAsync(policies);
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return policyWrapper.ExecuteAsync(async () 
                => { return await base.SendAsync(request, cancellationToken); }
            );
        }

        private Policy[] CreatePolicies() => new Policy[]
            {
                Policy.Handle<HttpRequestException>().WaitAndRetryAsync(
                    6, // Number of retries
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), // Exponential backoff
                    (exception, timeSpan, retryCount, context) => // On retry
                        {
                            var msg = $"Retry {retryCount} of {context.PolicyKey} at {context.ExecutionKey} due to: {exception}.";
                            logger.LogWarning(msg);
                        }
                ),
                Policy.Handle<HttpRequestException>().CircuitBreakerAsync(
                    5, // Number of exceptions before breaking circuit
                    TimeSpan.FromMinutes(1), // Time circuit opened before retry
                    (exception, duration) => // On break
                        {
                            logger.LogTrace("Circuit breaker opened");
                        },
                    () => // On reset
                        {
                            logger.LogTrace("Circuit breaker reset");
                        }
                 )
            };
    }
}
