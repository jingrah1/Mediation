using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Mediation.Defaults;

namespace Mediation {
    public class DefaultMediator : IMediator {
        MediationServiceProvider Services { get; }

        ConcurrentDictionary<Type, RequestWrapper> RequestWrappers =
            new ConcurrentDictionary<Type, RequestWrapper>();
        ConcurrentDictionary<Type, NotificationWrapper> NotificationWrappers =
            new ConcurrentDictionary<Type, NotificationWrapper>();

        public DefaultMediator(
            MediationServiceProvider services) {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        RequestWrapper GetOrAddRequestWrapper<TResult>(object request) {
            return RequestWrappers.GetOrAdd(request.GetType(), type => {
                return (RequestWrapper)Activator.CreateInstance(
                    typeof(RequestWrapper<,>).MakeGenericType(type, typeof(TResult)));
            });
        }

        public TResult Request<TResult>(object request) {
            var wrapper = GetOrAddRequestWrapper<TResult>(request);
            return (TResult)wrapper.Request(Services, request);
        }

        public async Task<TResult> RequestAsync<TResult>(object request, CancellationToken cancellationToken) {
            var wrapper = GetOrAddRequestWrapper<TResult>(request);
            cancellationToken.ThrowIfCancellationRequested();
            return (TResult) await wrapper.RequestAsync(Services, request, cancellationToken)
                .ConfigureAwait(false);
        }

        NotificationWrapper GetOrAddNotificationWrapper(object message) {
            return NotificationWrappers.GetOrAdd(message.GetType(), type => {
                return (NotificationWrapper)Activator.CreateInstance(
                    typeof(NotificationWrapper<>).MakeGenericType(type));
            });
        }

        public void Send(object message) {
            var wrapper = GetOrAddNotificationWrapper(message);
            wrapper.Send(Services, message);
        }

        public async Task SendAsync(object message, CancellationToken cancellationToken) {
            var wrapper = GetOrAddNotificationWrapper(message);
            cancellationToken.ThrowIfCancellationRequested();
            await wrapper.SendAsync(Services, message, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
