namespace TraceWPF.DI
{
    /// <summary>
    /// 单例生命周期标记接口。实现此接口的类将被 DI 容器注册为单例（Singleton）。
    /// Singleton lifetime marker interface. Classes implementing this interface are registered as singletons in the DI container.
    /// </summary>
    public interface ISingleton { }

    /// <summary>
    /// 作用域生命周期标记接口。实现此接口的类将被 DI 容器注册为作用域（Scoped）。
    /// Scoped lifetime marker interface. Classes implementing this interface are registered as scoped in the DI container.
    /// </summary>
    public interface IScoped { }

    /// <summary>
    /// 瞬态生命周期标记接口。实现此接口的类将被 DI 容器注册为瞬态（Transient）。
    /// Transient lifetime marker interface. Classes implementing this interface are registered as transient in the DI container.
    /// </summary>
    public interface ITransient { }
}

