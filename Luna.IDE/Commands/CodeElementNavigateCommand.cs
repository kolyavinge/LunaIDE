using System;
using System.Collections.Generic;
using System.Windows.Input;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;
using Luna.ProjectModel;

namespace Luna.IDE.Commands;

public interface ICodeElementNavigateCommand : ICommand { }

public class CodeElementNavigateCommandParameter
{
    public CodeElementNavigateCommandParameter(CodeFileProjectItem codeFile, CodeElement codeElement)
    {
        CodeFile = codeFile;
        CodeElement = codeElement;
    }

    public CodeFileProjectItem CodeFile { get; }
    public CodeElement CodeElement { get; }

    public override bool Equals(object? obj)
    {
        return obj is CodeElementNavigateCommandParameter parameter &&
               EqualityComparer<CodeFileProjectItem>.Default.Equals(CodeFile, parameter.CodeFile) &&
               EqualityComparer<CodeElement>.Default.Equals(CodeElement, parameter.CodeElement);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(CodeFile, CodeElement);
    }
}

public class CodeElementNavigateCommand : Command, ICodeElementNavigateCommand
{
    private readonly IEnvironmentWindowsManager _windowsManager;
    private readonly IProjectItemOpenCommand _projectItemOpenCommand;

    public CodeElementNavigateCommand(
        IEnvironmentWindowsManager windowsManager,
        IProjectItemOpenCommand projectItemOpenCommand)
    {
        _windowsManager = windowsManager;
        _projectItemOpenCommand = projectItemOpenCommand;
    }

    public override void Execute(object parameter)
    {
        var param = (CodeElementNavigateCommandParameter)parameter;
        _projectItemOpenCommand.Execute(new[] { param.CodeFile });
        var window = _windowsManager.FindWindowById(param.CodeFile) ?? throw new NullReferenceException();
        if (window.IsLoaded)
        {
            NavigateTo(window, param.CodeElement);
        }
        else
        {
            void OnWindowLoaded(object? sender, EventArgs e)
            {
                NavigateTo(window, param.CodeElement);
                window.Loaded -= OnWindowLoaded;
            }
            window.Loaded += OnWindowLoaded;
        }
    }

    private void NavigateTo(EnvironmentWindow window, CodeElement codeElement)
    {
        ((ICodeFileEditor)window.Model).NavigateTo(codeElement);
    }
}
