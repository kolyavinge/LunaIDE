using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Factories;

public interface IEnvironmentWindowsFactory
{
    EnvironmentWindowComponents MakeWindowFor(Type modelType);
}

public class EnvironmentWindowsFactory : IEnvironmentWindowsFactory
{
    public EnvironmentWindowComponents MakeWindowFor(Type modelType)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes().Union(Assembly.LoadFrom("Luna.IDE.dll").GetTypes()).ToList();
        var model = MakeModel(types, modelType);
        var viewModel = MakeViewModel(types, modelType, model);
        var view = MakeView(types, modelType, viewModel);

        return new EnvironmentWindowComponents(model, view);
    }

    private IEnvironmentWindowModel MakeModel(IEnumerable<Type> types, Type modelType)
    {
        var modelTypeConstructorParams = new List<object>();
        foreach (var parameter in modelType.GetConstructors().First().GetParameters())
        {
            modelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var model = Activator.CreateInstance(modelType, modelTypeConstructorParams.ToArray()) as IEnvironmentWindowModel ?? throw new EnvironmentWindowsFactoryException();
        DependencyContainer.ResolveInjectedProperties(model);

        return model;
    }

    private object MakeViewModel(IEnumerable<Type> types, Type modelType, IEnvironmentWindowModel model)
    {
        var viewModelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EnvironmentWindowForAttribute>()?.ModelType == modelType &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType.IsInstanceOfType(model)) ?? throw new EnvironmentWindowsFactoryException();

        var viewModelTypeConstructorParams = new List<object> { model };
        foreach (var parameter in viewModelType.GetConstructors().First().GetParameters().Skip(1))
        {
            viewModelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var viewModel = Activator.CreateInstance(viewModelType, viewModelTypeConstructorParams.ToArray()) ?? throw new EnvironmentWindowsFactoryException();
        DependencyContainer.ResolveInjectedProperties(viewModel);

        return viewModel;
    }

    private IEnvironmentWindowView MakeView(IEnumerable<Type> types, Type modelType, object viewModel)
    {
        var viewType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EnvironmentWindowForAttribute>()?.ModelType == modelType &&
            type.GetConstructor(new[] { viewModel.GetType() }) != null) ?? throw new EnvironmentWindowsFactoryException();

        var fe = Activator.CreateInstance(viewType, viewModel) as FrameworkElement ?? throw new EnvironmentWindowsFactoryException();
        var view = new EnvironmentWindowView(fe);

        return view;
    }
}

public class EnvironmentWindowsFactoryException : Exception { }
