using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {
    internal static class TypeExtensions {
        public static bool IsConcrete(
            this Type type)
            => !type.IsInterface && !type.IsAbstract;

        public static bool IsOpenGeneric(
            this Type type)
            => type.IsGenericType || type.ContainsGenericParameters;

        public static IEnumerable<Type> GetBaseTypes(this Type type) {
            for (var current = type; current != typeof(object); current = current.BaseType)
                yield return current.BaseType;
        }

        public static bool MatchesGenericTypeDefinition(this Type type, Type template)
            => type.IsGenericType && type.GetGenericTypeDefinition() == template;

        public static IEnumerable<Type> FilterByGenericTypeDefinition(this IEnumerable<Type> types, Type template)
            => types.Where(type => type.MatchesGenericTypeDefinition(template));

        public static IEnumerable<Type> GetMatchingGenericTypeDefinitions(this Type type, Type template)
            => (template.IsInterface ? type.GetInterfaces() : type.GetBaseTypes())
                .FilterByGenericTypeDefinition(template);

        public static bool MatchesInterfaceTypeArguments(this Type type, Type template) {
            if (!template.IsInterface) throw new ArgumentException("Template not an interface", nameof(template));
            if (!type.IsInterface) type = type.GetInterface(template.Name);
            return type.GenericTypeArguments.SequenceEqual(template.GenericTypeArguments);
        }
    }
}
