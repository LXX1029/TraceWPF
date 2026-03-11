namespace TraceWPF.DI
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// DI 服务注册扩展方法集合，提供基于标记接口的自动注册功能。
    /// DI service registration extension methods, providing automatic registration based on marker interfaces.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 自动扫描指定程序集中匹配命名空间前缀的类型，根据其实现的标记接口
        /// （ISingleton / IScoped / ITransient）自动注册到 DI 容器。
        /// 对于 Views 和 ViewModels 命名空间下的类型，会额外注册其具体类型以支持直接解析。
        /// 
        /// Automatically scans types in the specified assembly that match namespace prefixes,
        /// and registers them into the DI container based on their implemented marker interfaces
        /// (ISingleton / IScoped / ITransient).
        /// For types under the Views and ViewModels namespaces, their concrete types are also registered to support direct resolution.
        /// </summary>
        /// <param name="services">DI 服务集合 / The DI service collection.</param>
        /// <param name="assembly">要扫描的程序集 / The assembly to scan.</param>
        /// <param name="namespacePrefixes">命名空间前缀过滤器 / Namespace prefix filters.</param>
        public static void AutoRegister(this IServiceCollection services, Assembly assembly, params string[] namespacePrefixes)
        {
            var nsFilter = (namespacePrefixes ?? Array.Empty<string>())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToArray();

            /// <summary>
            /// 检查类型的命名空间是否匹配任意一个前缀过滤器。
            /// Checks whether a type's namespace matches any of the prefix filters.
            /// </summary>
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
                var concreteNs = new[] { "TraceWPF", "TraceWPF.Views", "TraceWPF.ViewModels" };
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
