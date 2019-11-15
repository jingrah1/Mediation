using Mediation;
using Mediation.Microsoft.Extensions.DependencyInjection.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services)
            where TMediator : class, IMediator
            => services.AddMediator<TMediator>(options => { });

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            Action<MediatorServiceConfiguration<TMediator>> optionsBuilder)
            where TMediator : class, IMediator
            => services.AddMediator(optionsBuilder, AppDomain.CurrentDomain.GetAssemblies());

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            params Assembly[] assemblies)
            where TMediator : class, IMediator
            => services.AddMediator<TMediator>(assemblies.AsEnumerable());

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            IEnumerable<Assembly> assemblies)
            where TMediator : class, IMediator
            => services.AddMediator<TMediator>(options => { }, assemblies);

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            Action<MediatorServiceConfiguration<TMediator>> optionsBuilder,
            params Assembly[] assemblies)
            where TMediator : class, IMediator
            => services.AddMediator(optionsBuilder, assemblies.AsEnumerable());

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            Action<MediatorServiceConfiguration<TMediator>> optionsBuilder,
            IEnumerable<Assembly> assemblies)
            where TMediator : class, IMediator
            => services.AddMediator(optionsBuilder, assemblies.SelectMany(x => x.GetTypes()));

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            params Type[] types)
            where TMediator : class, IMediator
            => services.AddMediator<TMediator>(types.AsEnumerable());

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            IEnumerable<Type> types)
            where TMediator : class, IMediator
            => services.AddMediator<TMediator>(options => { }, types);

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            Action<MediatorServiceConfiguration<TMediator>> optionsBuilder,
            params Type[] types)
            where TMediator : class, IMediator
            => services.AddMediator(optionsBuilder, types.AsEnumerable());

        public static IServiceCollection AddMediator<TMediator>(
            this IServiceCollection services,
            Action<MediatorServiceConfiguration<TMediator>> optionsBuilder,
            IEnumerable<Type> types)
            where TMediator : class, IMediator {
            var configuration = new MediatorServiceConfiguration<TMediator>();
            optionsBuilder(configuration);
            services
                .AddTransient<MediationServiceProvider>(provider => provider.GetService)
                .Add(new ServiceDescriptor(typeof(IMediator), configuration.Implementation, configuration.Lifetime));
            return services
                .AddMediationClasses(types);
        }



        public static IServiceCollection AddMediator(
            this IServiceCollection services) {
            return services
                .AddTransient<MediationServiceProvider>(provider => provider.GetService);
        }
        #endregion

        static IServiceCollection AddMediationClasses(
            this IServiceCollection services,
            IEnumerable<Type> types) {
            var concretions = types.Where(x => x.IsConcrete() && !x.IsOpenGeneric());
            foreach (var concretion in concretions) {
                foreach (var search in TypeSearches) {
                    var scanned = concretion
                        .GetMatchingGenericTypeDefinitions(search.Key)
                        .Where(x => x.IsInterface)
                        .Where(x => search.Value.AllowMultipleHandlers 
                            || concretion.MatchesInterfaceTypeArguments(x));
                    foreach (var entry in scanned) {
                        if (search.Value.AllowMultipleHandlers) services.AddTransient(entry, concretion);
                        else services.TryAddTransient(entry, concretion);
                    }
                }
            } return services;
        }
    }
}
