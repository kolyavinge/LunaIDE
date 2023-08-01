using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Luna.IDE.WindowsManagement;
using Luna.ProjectModel;

namespace Luna.IDE.App.Factories;

public class ProjectItemEditorFactory : WindowsFactory, IProjectItemEditorFactory
{
    public EnvironmentWindowComponents MakeEditorFor(ProjectItem projectItem)
    {
        var types = GetAvailableTypes();

        var modelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType == projectItem.GetType()) ?? throw new WindowsFactoryException();

        var model = MakeModel(modelType, new List<object> { projectItem });

        var viewModelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType.IsInstanceOfType(model)) ?? throw new WindowsFactoryException();

        var viewModel = MakeViewModel(viewModelType, model);

        var viewType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructor(new[] { viewModel.GetType() }) != null) ?? throw new WindowsFactoryException();

        var view = MakeView(viewType, viewModel);

        return new EnvironmentWindowComponents(model, view);
    }
}
