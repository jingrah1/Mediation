using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mediation {
    public interface IPipelineStep<in TRequest> {
        void Execute(TRequest request, Action next);

        Task ExecuteAsync(TRequest request, Func<Task> next, CancellationToken cancellationToken);
    }

    public interface IPipelineStep<TRequest, TResponse> {
        TResponse Execute(TRequest request, Func<TResponse> next);

        Task<TResponse> ExecuteAsync(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
    }
}
