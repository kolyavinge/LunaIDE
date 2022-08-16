using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Luna.IDE.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.Model;

public interface IProjectItemEditorFactory
{
    EnvironmentWindowComponents MakeEditorFor(ProjectItem projectItem);
}

[AttributeUsage(AttributeTargets.Class)]
public class EditorForAttribute : Attribute
{
    public Type ProjectItemType { get; }

    public EditorForAttribute(Type projectItemType)
    {
        ProjectItemType = projectItemType;
    }
}

public class ProjectItemEditorFactory : IProjectItemEditorFactory
{
    public EnvironmentWindowComponents MakeEditorFor(ProjectItem projectItem)
    {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var model = MakeModel(types, projectItem);
        var viewModel = MakeViewModel(types, projectItem, model);
        var view = MakeView(types, projectItem, viewModel);

        return new EnvironmentWindowComponents(model, view);
    }

    private IEnvironmentWindowModel MakeModel(IEnumerable<Type> types, ProjectItem projectItem)
    {
        var modelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType == projectItem.GetType()) ?? throw new ProjectItemEditorFactoryException();

        var modelTypeConstructorParams = new List<object> { projectItem };
        foreach (var parameter in modelType.GetConstructors().First().GetParameters().Skip(1))
        {
            modelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var model = Activator.CreateInstance(modelType, modelTypeConstructorParams.ToArray()) as IEnvironmentWindowModel ?? throw new ProjectItemEditorFactoryException();
        DependencyContainer.ResolveInjectedProperties(model);

        return model;
    }

    private object MakeViewModel(IEnumerable<Type> types, ProjectItem projectItem, IEnvironmentWindowModel model)
    {
        var viewModelType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructors().Length == 1 &&
            type.GetConstructors().First().GetParameters().Length > 0 &&
            type.GetConstructors().First().GetParameters().First().ParameterType.IsInstanceOfType(model)) ?? throw new ProjectItemEditorFactoryException();

        var viewModelTypeConstructorParams = new List<object> { model };
        foreach (var parameter in viewModelType.GetConstructors().First().GetParameters().Skip(1))
        {
            viewModelTypeConstructorParams.Add(DependencyContainer.Resolve(parameter.ParameterType));
        }

        var viewModel = Activator.CreateInstance(viewModelType, viewModelTypeConstructorParams.ToArray()) ?? throw new ProjectItemEditorFactoryException();
        DependencyContainer.ResolveInjectedProperties(viewModel);

        return viewModel;
    }

    private object MakeView(IEnumerable<Type> types, ProjectItem projectItem, object viewModel)
    {
        var viewType = types.FirstOrDefault(type =>
            type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
            type.GetConstructor(new[] { viewModel.GetType() }) != null) ?? throw new ProjectItemEditorFactoryException();

        var view = Activator.CreateInstance(viewType, viewModel) ?? throw new ProjectItemEditorFactoryException();

        return view;
    }
}

public class ProjectItemEditorFactoryException : Exception { }
