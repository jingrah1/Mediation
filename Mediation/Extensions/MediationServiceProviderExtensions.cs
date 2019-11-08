using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mediation {
    public static class MediationServiceProviderExtensions {
        public static T GetService<T>(this MediationServiceProvider serviceProvider)
            => (T)serviceProvider(typeof(T));

        public static IEnumerable<T> GetServices<T>(this MediationServiceProvider serviceProvider)
            => (IEnumerable<T>)serviceProvider(typeof(IEnumerable<T>));


        public static Func<THandler> GenerateRequestPipeline<TRequest, TResult, THandler>(
            this MediationServiceProvider serviceProvider,
            Func<IRequestHandler<TRequest, TResult>, Func<THandler>> seed,
            Func<Func<THandler>, IPipelineStep<TRequest, TResult>, Func<THandler>> method) {
            var handler = serviceProvider.GetService<IRequestHandler<TRequest, TResult>>();
            return serviceProvider.GetServices<IPipelineStep<TRequest, TResult>>()
                .Reverse()
                .Aggregate(seed(handler), method);
        }

        public static Action GenerateNotificationPipeline<TRequest>(
            this MediationServiceProvider serviceProvider,
            Func<INotificationHandler<TRequest>, Action> seed,
            Func<Action, IPipelineStep<TRequest>, Action> method) {
            var handlers = serviceProvider.GetServices<INotificationHandler<TRequest>>();
            return serviceProvider.GetServices<IPipelineStep<TRequest>>()
                .Reverse()
                .Aggregate(
                    () => { foreach (var handler in handlers) seed(handler)(); }, 
                    method);
        }

        public static Func<Task> GenerateNotificationPipeline<TRequest>(
            this MediationServiceProvider serviceProvider,
            Func<INotificationHandler<TRequest>, Func<Task>> seed,
            Func<Func<Task>, IPipelineStep<TRequest>, Func<Task>> method) {
            var handlers = serviceProvider.GetServices<INotificationHandler<TRequest>>();
            return serviceProvider.GetServices<IPipelineStep<TRequest>>()
                .Reverse()
                .Aggregate(
                    () => Task.WhenAll(handlers.Select(x => seed(x)())),
                    method);
        }
    }
}
