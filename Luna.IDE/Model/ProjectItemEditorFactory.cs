using System;
using System.Linq;
using System.Reflection;
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

        var modelType = types
            .FirstOrDefault(type =>
                type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
                type.GetConstructor(new[] { projectItem.GetType() }) != null) ?? throw new ProjectItemEditorFactoryException();

        var model = (Activator.CreateInstance(modelType, new[] { projectItem }) as IEnvironmentWindowModel) ?? throw new ProjectItemEditorFactoryException();

        var viewModelType = types
            .FirstOrDefault(type =>
                type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
                type.GetConstructor(new[] { model.GetType() }) != null) ?? throw new ProjectItemEditorFactoryException();

        var viewModel = Activator.CreateInstance(viewModelType, new[] { model }) ?? throw new ProjectItemEditorFactoryException();

        var viewType = types
            .FirstOrDefault(type =>
                type.GetCustomAttribute<EditorForAttribute>()?.ProjectItemType == projectItem.GetType() &&
                type.GetConstructor(new[] { viewModel.GetType() }) != null) ?? throw new ProjectItemEditorFactoryException();

        var view = Activator.CreateInstance(viewType, new[] { viewModel }) ?? throw new ProjectItemEditorFactoryException();

        return new EnvironmentWindowComponents(model, view);
    }
}

public class ProjectItemEditorFactoryException : Exception{}
