using System.Threading;
using System.Threading.Tasks;

namespace Mediation {
    public static class MediatorExtensions {
        public static TResult Request<TRequest, TResult>(
            this IMediator mediator,
            TRequest request)
            => mediator.Request<TResult>(request);

        public static Task<TResult> RequestAsync<TResult>(
            this IMediator mediator,
            object request)
            => mediator.RequestAsync<TResult>(request, CancellationToken.None);

        public static Task<TResult> RequestAsync<TRequest, TResult>(
            this IMediator mediator,
            TRequest request)
            => mediator.RequestAsync<TRequest, TResult>(request, CancellationToken.None);

        public static Task<TResult> RequestAsync<TRequest, TResult>(
            this IMediator mediator,
            TRequest request,
            CancellationToken cancellationToken)
            => mediator.RequestAsync<TResult>(request, cancellationToken);

        public static void Send<TMessage>(
            this IMediator mediator,
            TMessage message)
            => mediator.Send(message);

        public static Task SendAsync<TMessage>(
            this IMediator mediator,
            TMessage request)
            => mediator.SendAsync<TMessage>(request, CancellationToken.None);

        public static Task SendAsync<TMessage>(
            this IMediator mediator,
            TMessage message,
            CancellationToken cancellationToken)
            => mediator.SendAsync(message, cancellationToken);
    }
}
