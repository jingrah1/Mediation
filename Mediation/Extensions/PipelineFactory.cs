using System;

namespace Mediation.Extensions {
    public sealed class PipelineFactory {
        MediationServiceProvider ServiceProvider { get; }

        public PipelineFactory(MediationServiceProvider serviceProvider) {
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public void GenerateRequestPipeline<TRequest, TResult>(
            IRequestHandler<TRequest, TResult> handler) {
            var steps = ServiceProvider.GetServices<IPipelineStep<TRequest, TResult>>();
        }

        public void GenerateNotificationPipeline<TRequest>(
            INotificationHandler<TRequest> handler) {
            var steps = ServiceProvider.GetServices<IPipelineStep<TRequest>>();
        }
    }
}
