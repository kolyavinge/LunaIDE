﻿using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Controls.Tree;
using Luna.ProjectModel;
using Luna.Utils;

namespace Luna.IDE.Model;

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
        if (projecItem.Parent == null) return "project.png";
        else return "directory.png";
    }
}

public class CodeFileTreeItem : TreeItem
{
    private readonly HashSet<string> _lastSelectedNames = new();

    public CodeFileProjectItem CodeFile { get; }

    public CodeFileTreeItem(DirectoryTreeItem parent, CodeFileProjectItem codeFile) :
        base(parent, codeFile.Name, "codefile.png")
    {
        CodeFile = codeFile;
        CodeFile.CodeModelUpdated += (s, e) =>
        {
            _lastSelectedNames.Clear();
            _lastSelectedNames.AddRange(Children.Where(x => x.IsSelected).Select(x => x.Name).ToList());
            UpdateChildren();
        };
    }

    protected override IEnumerable<TreeItem> GetChildren()
    {
        foreach (var constant in CodeFile.CodeModel.Constants)
        {
            yield return new CodeElementTreeItem(this, constant) { IsSelected = _lastSelectedNames.Contains(constant.Name) };
        }
        foreach (var func in CodeFile.CodeModel.Functions)
        {
            yield return new CodeElementTreeItem(this, func) { IsSelected = _lastSelectedNames.Contains(func.Name) };
        }
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

    protected override IEnumerable<TreeItem> GetChildren()
    {
        return Enumerable.Empty<TreeItem>();
    }
}
