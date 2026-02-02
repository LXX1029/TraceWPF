namespace TraceWPF.DI
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    public static class ServiceCollectionExtensions
    {
        public static void AutoRegister(this IServiceCollection services, Assembly assembly, params string[] namespacePrefixes)
        {
            var nsFilter = (namespacePrefixes ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            bool MatchNamespace(Type t)
            {
                if (nsFilter.Length == 0) return true;
                var ns = t.Namespace ?? "";
                return nsFilter.Any(p => ns.StartsWith(p, StringComparison.Ordinal));
            }

            var markerTypes = new[] { typeof(ISingleton), typeof(IScoped), typeof(ITransient) };
            var types = assembly.GetTypes()
                                .Where(t => t.IsClass && !t.IsAbstract && MatchNamespace(t));

            foreach (var type in types)
            {
                var lifetimeMarker = type.GetInterfaces().FirstOrDefault(i => markerTypes.Contains(i));
                if (lifetimeMarker == null) continue;

                var serviceInterfaces = type.GetInterfaces().Where(i => !markerTypes.Contains(i)).ToArray();
                var ns = type.Namespace ?? "";
                var concreteNs = new[] { "TraceWPF.Views", "TraceWPF.ViewModels" };
                var registerConcrete = concreteNs.Any(p => ns.StartsWith(p, StringComparison.Ordinal));

                if (lifetimeMarker == typeof(ISingleton))
                {
                    if (serviceInterfaces.Length > 0)
                        foreach (var i in serviceInterfaces) services.AddSingleton(i, type);
                    if (registerConcrete || serviceInterfaces.Length == 0)
                        services.AddSingleton(type);
                }
                else if (lifetimeMarker == typeof(IScoped))
                {
                    if (serviceInterfaces.Length > 0)
                        foreach (var i in serviceInterfaces) services.AddScoped(i, type);
                    if (registerConcrete || serviceInterfaces.Length == 0)
                        services.AddScoped(type);
                }
                else if (lifetimeMarker == typeof(ITransient))
                {
                    if (serviceInterfaces.Length > 0)
                        foreach (var i in serviceInterfaces) services.AddTransient(i, type);
                    if (registerConcrete || serviceInterfaces.Length == 0)
                        services.AddTransient(type);
                }
            }
        }
    }
}
