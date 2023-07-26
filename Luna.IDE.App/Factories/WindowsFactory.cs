using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using Luna.IDE.App.Infrastructure;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Factories;

public abstract class WindowsFactory
{
    protected List<Type> GetAvailableTypes()
    {
        return Assembly.GetExecutingAssembly().GetTypes().Union(Assembly.LoadFrom("Luna.IDE.dll").GetTypes()).ToList();
    }

    protected IEnvironmentWindowModel MakeModel(Type modelType, List<object> modelTypeConstructorParams)
    {
        var constuctorParameters = modelType.GetConstructors().First().GetParameters().Skip(modelTypeConstructorParams.Count).ToList();
        foreach (var parameter in constuctorParameters)
        {
            modelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var model = Activator.CreateInstance(modelType, modelTypeConstructorParams.ToArray()) as IEnvironmentWindowModel ?? throw new WindowsFactoryException();
        DependencyContainer.ResolveInjectedProperties(model);

        return model;
    }

    protected object MakeViewModel(Type viewModelType, IEnvironmentWindowModel model)
    {
        var viewModelTypeConstructorParams = new List<object> { model };
        foreach (var parameter in viewModelType.GetConstructors().First().GetParameters().Skip(1))
        {
            viewModelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var viewModel = Activator.CreateInstance(viewModelType, viewModelTypeConstructorParams.ToArray()) ?? throw new WindowsFactoryException();
        DependencyContainer.ResolveInjectedProperties(viewModel);

        return viewModel;
    }

    protected IEnvironmentWindowView MakeView(Type viewType, object viewModel)
    {
        var fe = Activator.CreateInstance(viewType, viewModel) as FrameworkElement ?? throw new WindowsFactoryException();
        var view = new EnvironmentWindowView(fe);

        return view;
    }
}

public class WindowsFactoryException : Exception { }
