using System.Collections;
using System.Linq;
using System.Windows.Input;
using Luna.IDE.Controls.Tree;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.Commands;

public interface IProjectExplorerItemOpenCommand : ICommand { }

public class ProjectExplorerItemOpenCommand : Command, IProjectExplorerItemOpenCommand
{
    private readonly IProjectItemOpenCommand _projectItemOpenCommand;
    private readonly ICodeElementNavigateCommand _codeElementNavigateCommand;

    public ProjectExplorerItemOpenCommand(
        IProjectItemOpenCommand projectItemOpenCommand,
        ICodeElementNavigateCommand codeElementNavigateCommand)
    {
        _projectItemOpenCommand = projectItemOpenCommand;
        _codeElementNavigateCommand = codeElementNavigateCommand;
    }

    public override void Execute(object parameter)
    {
        var treeItems = ((IEnumerable)parameter).Cast<TreeItem>().Where(x => x.Parent != null).ToList();
        if (!treeItems.Any()) return;
        if (treeItems.Count == 1 && treeItems.First() is DirectoryTreeItem)
        {
            var directory = treeItems.First();
            directory.IsExpanded = !directory.IsExpanded;
        }
        else if (treeItems.OfType<CodeElementTreeItem>().Any())
        {
            var codeElementTreeItem = treeItems.OfType<CodeElementTreeItem>().First();
            _codeElementNavigateCommand.Execute(new CodeElementNavigateCommandParameter(codeElementTreeItem.ParentCodeFile.CodeFile, codeElementTreeItem.CodeElement));
        }
        else
        {
            _projectItemOpenCommand.Execute(treeItems.OfType<CodeFileTreeItem>().Select(x => x.CodeFile));
        }
    }
}
