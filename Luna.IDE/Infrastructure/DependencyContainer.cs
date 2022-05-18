using System;

namespace Luna.IDE.Infrastructure;

public static class DependencyContainer
{
    private static readonly DependencyInjection.IDependencyContainer _container;

    static DependencyContainer()
    {
        _container = DependencyInjection.DependencyContainerFactory.MakeLiteContainer();
        _container.InitFromModules(new InjectModule());
    }

    public static TDependency Resolve<TDependency>()
    {
        return _container.Resolve<TDependency>();
    }

    public static void Dispose()
    {
        _container.Dispose();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class InjectAttribute : DependencyInjection.InjectAttribute { }
