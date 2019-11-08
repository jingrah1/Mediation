using System.Threading;
using System.Threading.Tasks;

namespace Mediation.Defaults {
    class RequestWrapper<TRequest, TResult> : RequestWrapper {
        public override object Request(MediationServiceProvider handlerProvider, object request) {
            var value = (TRequest)request;
            return handlerProvider.GenerateRequestPipeline<TRequest, TResult, TResult>(
                (handler) => () => handler.Handle(value),
                (next, step) => () => step.Execute(value, next));
        }

        public override async Task<object> RequestAsync(MediationServiceProvider handlerProvider, object request, CancellationToken cancellationToken) {
            var value = (TRequest)request;
            return await handlerProvider.GenerateRequestPipeline<TRequest, TResult, Task<TResult>>(
                (handler) => () => handler.HandleAsync(value, cancellationToken), 
                (next, step) => () => step.ExecuteAsync(value, next, cancellationToken))();
        }
    }

    abstract class RequestWrapper {
        public abstract object Request(MediationServiceProvider handlerProvider, object request);

        public abstract Task<object> RequestAsync(MediationServiceProvider handlerProvider, object requests, CancellationToken cancellationToken);
    }
}
