using System.Linq;
using System.Reflection;
using Luna.IDE.Infrastructure;

namespace Luna.IDE.App.Infrastructure;

public static class DependencyContainer
{
    private static readonly DependencyInjection.IDependencyContainer _container;

    static DependencyContainer()
    {
        _container = DependencyInjection.DependencyContainerFactory.MakeLiteContainer();
        _container.InitFromModules(new RuntimeInjectModule());
        _container.InitFromModules(new IDEInjectModule());
        _container.InitFromModules(new AppInjectModule());
    }

    public static TDependency Resolve<TDependency>()
    {
        return _container.Resolve<TDependency>();
    }

    public static object Resolve(Type dependencyType)
    {
        return _container.Resolve(dependencyType);
    }

    public static void ResolveInjectedProperties(object obj)
    {
        var properties = obj.GetType().GetProperties().Where(prop => prop.GetCustomAttribute<InjectAttribute>() != null).ToList();
        foreach (var prop in properties)
        {
            prop.SetValue(obj, _container.Resolve(prop.PropertyType));
        }
    }

    public static void Dispose()
    {
        _container.Dispose();
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class InjectAttribute : DependencyInjection.InjectAttribute { }
