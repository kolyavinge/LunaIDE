using Luna.ProjectModel;

namespace Luna.IDE.WindowsManagement;

public interface IProjectItemEditorFactory
{
    EnvironmentWindowComponents MakeEditorFor(ProjectItem projectItem);
}
