using Mediation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection {
    public static class ServiceCollectionExtensions {
        struct TypeSearchConfiguration {
            public bool AllowMultipleHandlers { get; }

            public TypeSearchConfiguration(
                bool allowMultipleHandlers) {
                AllowMultipleHandlers = allowMultipleHandlers;
            }
        }

        static IReadOnlyDictionary<Type, TypeSearchConfiguration> TypeSearches =
            new Dictionary<Type, TypeSearchConfiguration> {
                { typeof(IRequestHandler<,>), new TypeSearchConfiguration(false) },
                { typeof(INotificationHandler<>), new TypeSearchConfiguration(true) }
            };

        #region AddMediator
        public static IServiceCollection AddMediator(
            this IServiceCollection services) {
            return services
                .AddTransient<MediationServiceProvider>(provider => provider.GetService);
        }
        #endregion

        internal static IEnumerable<Type> ApplyMediationFilter(
            this IEnumerable<Type> types) {
            var concretions = types.Where(x => x.IsConcrete());
            throw new NotImplementedException();
        }
    }
}
