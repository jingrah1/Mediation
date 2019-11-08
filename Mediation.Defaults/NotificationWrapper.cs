using System.Threading;
using System.Threading.Tasks;

namespace Mediation.Defaults {
    class NotificationWrapper<TMessage> : NotificationWrapper {
        public override void Send(
            MediationServiceProvider serviceProvider, 
            object message) {
            var value = (TMessage)message;
            serviceProvider.GenerateNotificationPipeline<TMessage>(
                (handler) => () => handler.Handle(value),
                (next, step) => () => step.Execute(value, next))();
        }

        public override async Task SendAsync(
            MediationServiceProvider serviceProvider, 
            object message, 
            CancellationToken cancellationToken) {
            var value = (TMessage)message;
            await serviceProvider.GenerateNotificationPipeline<TMessage>(
                (handler) => () => handler.HandleAsync(value, cancellationToken),
                (next, step) => () => step.ExecuteAsync(value, next, cancellationToken))();
        }
    }

    abstract class NotificationWrapper {
        public abstract void Send(MediationServiceProvider serviceProvider, object message);

        public abstract Task SendAsync(MediationServiceProvider serviceProvider, object message, CancellationToken cancellationToken);
    }
}
