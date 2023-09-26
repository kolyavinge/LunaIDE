using System.Collections.Generic;
using Luna.CodeElements;
using Luna.IDE.Common;
using Luna.ProjectModel;

namespace Luna.IDE.ProjectExploration;

public class DirectoryTreeItem : TreeItem
{
    public DirectoryProjectItem ProjecItem { get; }

    public DirectoryTreeItem(DirectoryTreeItem? parent, DirectoryProjectItem projectItem) :
        base(parent, projectItem.Name, GetImageFileName(projectItem))
    {
        ProjecItem = projectItem;
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        foreach (var item in ProjecItem.Children)
        {
            if (item is DirectoryProjectItem directory) yield return new DirectoryTreeItem(this, directory);
            if (item is CodeFileProjectItem codeFile) yield return new CodeFileTreeItem(this, codeFile);
        }
    }

    private static string GetImageFileName(ProjectItem projecItem)
    {
        if (projecItem.Parent is null) return "project.png";
        else return "directory.png";
    }
}

public class CodeFileTreeItem : TreeItem
{
    public CodeFileProjectItem CodeFile { get; }

    public CodeFileTreeItem(DirectoryTreeItem parent, CodeFileProjectItem codeFile) :
        base(parent, codeFile.Name, "codefile.png")
    {
        CodeFile = codeFile;
        CodeFile.CodeModelUpdated += OnCodeModelUpdated;
    }

    private void OnCodeModelUpdated(object? sender, CodeModelUpdatedEventArgs e)
    {
        if (!e.OldCodeModel.Equals(CodeFile.CodeModel))
        {
            RefreshChildren();
        }
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        foreach (var constant in CodeFile.CodeModel.Constants) yield return new CodeElementTreeItem(this, constant);
        foreach (var func in CodeFile.CodeModel.Functions) yield return new CodeElementTreeItem(this, func);
    }
}

public class CodeElementTreeItem : TreeItem
{
    public CodeFileTreeItem ParentCodeFile { get; }

    public CodeElement CodeElement { get; }

    public CodeElementTreeItem(CodeFileTreeItem parentCodeFile, ConstantDeclaration constantDeclaration) :
        base(parentCodeFile, constantDeclaration.Name, "const.png")
    {
        ParentCodeFile = parentCodeFile;
        CodeElement = constantDeclaration;
    }

    public CodeElementTreeItem(CodeFileTreeItem parentCodeFile, FunctionDeclaration functionDeclaration) :
        base(parentCodeFile, functionDeclaration.Name, "func.png")
    {
        ParentCodeFile = parentCodeFile;
        CodeElement = functionDeclaration;
    }
}
