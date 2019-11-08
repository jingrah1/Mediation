using System.Threading;
using System.Threading.Tasks;

namespace Mediation {
    public interface IMediator {
        TResult Request<TResult>(object request);

        Task<TResult> RequestAsync<TResult>(object request, CancellationToken cancellationToken);

        void Send(object message);

        Task SendAsync(object message, CancellationToken cancellationToken);
    }
}
