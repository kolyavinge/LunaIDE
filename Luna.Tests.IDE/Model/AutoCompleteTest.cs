using System;
using System.Linq;
using Luna.IDE.Model;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;
using Token = CodeHighlighter.Model.Token;

namespace Luna.Tests.IDE.Model;

public class AutoCompleteTest
{
    private CodeModel _mainCodeModel;
    private Mock<IAutoCompleteDataContext> _dataContext;
    private AutoComplete _autoComplete;

    [SetUp]
    public void Setup()
    {
        _mainCodeModel = new CodeModel();
        _dataContext = new Mock<IAutoCompleteDataContext>();
        _dataContext.Setup(x => x.CodeModel).Returns(_mainCodeModel);
        _autoComplete = new AutoComplete();
        _autoComplete.Init(_dataContext.Object);
    }

    [Test]
    public void Show()
    {
        var importedCodeModel = new CodeModel();
        importedCodeModel.AddConstantDeclaration(new("IMPORTED_WIDTH", new IntegerValueElement(1)));
        importedCodeModel.AddFunctionDeclaration(new("my_imported_func", new FunctionArgument[0], new()));
        _mainCodeModel.AddConstantDeclaration(new("WIDTH", new IntegerValueElement(1)));
        _mainCodeModel.AddFunctionDeclaration(new("my_func", new FunctionArgument[0], new()));
        _mainCodeModel.AddImportDirective(new("", new("", null, null) { CodeModel = importedCodeModel }));

        _autoComplete.Show();

        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "lambda"));
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "and"));
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "WIDTH"));
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "my_func"));
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "IMPORTED_WIDTH"));
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "my_imported_func"));
        Assert.Null(_autoComplete.SelectedItem);
        Assert.True(_autoComplete.IsVisible);
    }

    [Test]
    public void Show_Filtered_FullTokenName()
    {
        _dataContext.SetupGet(x => x.CursorPosition).Returns((10, 6));
        var token = new Token("lambda", 0, 6, (byte)TokenKind.Lambda);
        _dataContext.Setup(x => x.GetTokenOnPosition(10, 6)).Returns(token);

        _autoComplete.Show();

        Assert.AreEqual(1, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "lambda"));
        Assert.NotNull(_autoComplete.SelectedItem);
        Assert.True(_autoComplete.SelectedItem.Name == "lambda");
    }

    [Test]
    public void Show_Filtered_ShortTokenName()
    {
        _dataContext.SetupGet(x => x.CursorPosition).Returns((10, 4));
        _dataContext.Setup(x => x.GetTokenOnPosition(10, 4)).Returns(new Token("lambda", 0, 6, (byte)TokenKind.Lambda));

        _autoComplete.Show();

        Assert.AreEqual(1, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "lambda"));
        Assert.NotNull(_autoComplete.SelectedItem);
        Assert.True(_autoComplete.SelectedItem.Name == "lambda");
    }

    [Test]
    public void Show_Filtered_IgnoreCase()
    {
        _dataContext.SetupGet(x => x.CursorPosition).Returns((10, 6));
        _dataContext.Setup(x => x.GetTokenOnPosition(10, 6)).Returns(new Token("LAMBDA", 0, 6, (byte)TokenKind.Lambda));

        _autoComplete.Show();

        Assert.AreEqual(1, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.Items.FirstOrDefault(x => x.Name == "lambda"));
        Assert.NotNull(_autoComplete.SelectedItem);
        Assert.True(_autoComplete.SelectedItem.Name == "lambda");
    }

    [Test]
    public void TextChanged()
    {
        _mainCodeModel.AddConstantDeclaration(new("zzz", new IntegerValueElement(1)));
        _mainCodeModel.AddConstantDeclaration(new("zzz1", new IntegerValueElement(1)));
        _mainCodeModel.AddConstantDeclaration(new("zzz12", new IntegerValueElement(1)));

        _autoComplete.Show();

        _dataContext.SetupGet(x => x.CursorPosition).Returns((0, 3));
        _dataContext.Setup(x => x.GetTokenOnPosition(0, 3)).Returns(new Token("zzz", 0, 3, (byte)TokenKind.Identificator));
        _dataContext.Raise(x => x.TextChanged += null, EventArgs.Empty);
        Assert.AreEqual(3, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.SelectedItem.Name == "zzz");

        _dataContext.SetupGet(x => x.CursorPosition).Returns((0, 4));
        _dataContext.Setup(x => x.GetTokenOnPosition(0, 4)).Returns(new Token("zzz1", 0, 4, (byte)TokenKind.Identificator));
        _dataContext.Raise(x => x.TextChanged += null, EventArgs.Empty);
        Assert.AreEqual(2, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.SelectedItem.Name == "zzz1");

        _dataContext.SetupGet(x => x.CursorPosition).Returns((0, 5));
        _dataContext.Setup(x => x.GetTokenOnPosition(0, 5)).Returns(new Token("zzz12", 0, 5, (byte)TokenKind.Identificator));
        _dataContext.Raise(x => x.TextChanged += null, EventArgs.Empty);
        Assert.AreEqual(1, _autoComplete.Items.Count());
        Assert.NotNull(_autoComplete.SelectedItem.Name == "zzz12");
    }

    [Test]
    public void Complete_NoKeyboardInput()
    {
        _autoComplete.Show();
        _autoComplete.SelectedItem = _autoComplete.Items.First(x => x.Name == "lambda");

        _autoComplete.Complete();

        _dataContext.Verify(x => x.ReplaceText(0, 0, 0, 0, "lambda"), Times.Once());
        Assert.False(_autoComplete.IsVisible);
    }

    [Test]
    public void Complete_WithKeyboardInput()
    {
        _dataContext.SetupGet(x => x.CursorPosition).Returns((0, 6));
        _dataContext.Setup(x => x.GetTokenOnPosition(0, 6)).Returns(new Token("lambda", 0, 6, (byte)TokenKind.Lambda));
        _autoComplete.Show();

        _autoComplete.Complete();

        _dataContext.Verify(x => x.ReplaceText(0, 0, 0, 6, "lambda"), Times.Once());
    }

    [Test]
    public void Complete_NoSelection_NoCompletedEventRaise()
    {
        var completed = false;
        _autoComplete.Completed += (s, e) => completed = true;
        _autoComplete.Show();
        _autoComplete.SelectedItem = null;

        _autoComplete.Complete();

        Assert.False(completed);
        Assert.False(_autoComplete.IsVisible);
    }

    [Test]
    public void Complete_WithSelection_CompletedEventRaised()
    {
        var completed = false;
        _autoComplete.Completed += (s, e) => completed = true;
        _autoComplete.Show();
        _autoComplete.SelectedItem = _autoComplete.Items.First();

        _autoComplete.Complete();

        Assert.True(completed);
        Assert.False(_autoComplete.IsVisible);
    }
}
