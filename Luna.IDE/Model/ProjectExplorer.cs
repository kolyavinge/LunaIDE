using System;
using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Infrastructure;
using Luna.Infrastructure;
using Luna.ProjectModel;

namespace Luna.IDE.Model
{
    public interface IProjectExplorer
    {
        Project? Project { get; }
        ProjectTreeItem? ProjectTreeRoot { get; }
        IEnumerable<ProjectTreeItem> SelectedItems { get; }
        void OpenProject(string path);

        event EventHandler? ProjectOpen;
    }

    public class ProjectExplorer : IProjectExplorer
    {
        [Inject]
        public IFileSystem? FileSystem { get; set; }

        public Project? Project { get; private set; }

        public ProjectTreeItem? ProjectTreeRoot { get; private set; }

        public IEnumerable<ProjectTreeItem> SelectedItems => ProjectTreeRoot!.GetAllChildren().Where(x => x.IsSelected);

        public event EventHandler? ProjectOpen;

        public ProjectExplorer()
        {
        }

        public void OpenProject(string path)
        {
            Project = Project.Open(path, FileSystem!);
            ProjectTreeRoot = new ProjectTreeItem(Project.Root);
            ProjectOpen?.Invoke(this, EventArgs.Empty);
        }
    }
}
