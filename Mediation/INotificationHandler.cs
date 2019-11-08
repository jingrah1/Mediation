using System.Threading;
using System.Threading.Tasks;

namespace Mediation {
    public interface INotificationHandler<in TMessage> {
        void Handle(TMessage request);

        Task HandleAsync(TMessage request, CancellationToken cancellationToken);
    }
}
