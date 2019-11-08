using System.Threading;
using System.Threading.Tasks;

namespace Mediation {
    public interface IRequestHandler<in TRequest, TResult> {
        TResult Handle(TRequest request);

        Task<TResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}
