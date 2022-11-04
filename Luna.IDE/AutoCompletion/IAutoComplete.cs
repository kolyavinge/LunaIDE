using System.Collections.Generic;

namespace Luna.IDE.AutoCompletion;

public interface IAutoComplete
{
    event EventHandler? Completed;

    IReadOnlyCollection<IAutoCompleteItem> Items { get; set; }

    public IAutoCompleteItem? SelectedItem { get; set; }

    int SelectedIndex { get; set; }

    bool IsVisible { get; set; }

    IAutoCompleteDataContext DataContext { get; set; }

    void Init(IAutoCompleteDataContext dataContext);

    void Show();

    void Complete();

    void MoveSelectionUp();

    void MoveSelectionDown();

    void MoveSelectionPageUp(int pageSize);

    void MoveSelectionPageDown(int pageSize);

    CodeHighlighter.Model.Token? GetCursorToken();
}
