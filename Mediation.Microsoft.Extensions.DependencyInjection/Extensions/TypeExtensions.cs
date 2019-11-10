using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System {
    public static class TypeExtensions {
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

        public static IEnumerable<Type> MatchGenericTypeDefinitions(this Type type, Type template) {
            return (template.IsInterface ? type.GetInterfaces() : type.GetBaseTypes())
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == template);
        }

        public static bool MatchesInterfaceTypeArguments(this Type type, Type template) {
            if (!template.IsInterface) throw new ArgumentException("Type not an interface", nameof(template));
            if (!type.IsInterface) type = type.GetInterface(template.Name);
            return type.GenericTypeArguments.SequenceEqual(template.GenericTypeArguments);
        }

        public static bool TryMakeGenericType(this Type type, out Type? generic, params Type[] arguments) {
            try {
                generic = type.MakeGenericType(arguments);
                return true;
            } catch {
                generic = null;
                return false;
            }
        }
    }
}
