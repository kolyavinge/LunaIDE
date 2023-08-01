using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Luna.IDE.WindowsManagement;

namespace Luna.IDE.App.Factories;

public class EnvironmentWindowsFactory : WindowsFactory, IEnvironmentWindowsFactory
{
    public EnvironmentWindowComponents MakeWindowFor(Type modelType)
    {
        var types = GetAvailableTypes();

        var model = MakeModel(modelType, new List<object>());

        var viewModelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EnvironmentWindowForAttribute>()?.ModelType == modelType &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType.IsInstanceOfType(model)) ?? throw new WindowsFactoryException();

        var viewModel = MakeViewModel(viewModelType, model);

        var viewType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EnvironmentWindowForAttribute>()?.ModelType == modelType &&
            type.GetConstructor(new[] { viewModel.GetType() }) != null) ?? throw new WindowsFactoryException();

        var view = MakeView(viewType, viewModel);

        return new EnvironmentWindowComponents(model, view);
    }
}
