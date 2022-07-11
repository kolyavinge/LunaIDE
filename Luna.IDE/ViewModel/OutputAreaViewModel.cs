using System.Windows.Input;
using CodeHighlighter.Contracts;
using Luna.IDE.Model;
using Luna.IDE.Mvvm;

namespace Luna.IDE.ViewModel;

public class OutputAreaViewModel : NotificationObject
{
    public IOutputArea OutputArea { get; }

    public ICommand CodeTextBoxModelLoadedCommand { get; }

    public OutputAreaViewModel(IOutputArea outputArea)
    {
        OutputArea = outputArea;
        CodeTextBoxModelLoadedCommand = new ActionCommand<CodeTextBoxModel>(model => OutputArea.CodeTextBoxModel = model);
    }
}
