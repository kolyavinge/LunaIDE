using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel;

internal class CodeModelBuilderTest
{
    private CodeFileProjectItem _codeFile1, _codeFile2;
    private CodeFileProjectItem[] _allCodeFiles;
    private Mock<ICodeFileParsingContext> _codeFile1ParsingContext, _codeFile2ParsingContext;
    private Mock<ICodeFileParsingContextFactory> _contextFactory;
    private Mock<ICodeFileOrderLogic> _orderLogic;
    private Mock<ICodeModelUpdateRaiser> _codeModelUpdateRaiser;
    private Mock<IOutputWriter> _outputWriter;
    private CodeModelBuilder _builder;

    [SetUp]
    public void Setup()
    {
        _contextFactory = new Mock<ICodeFileParsingContextFactory>();
        _orderLogic = new Mock<ICodeFileOrderLogic>();
        _codeModelUpdateRaiser = new Mock<ICodeModelUpdateRaiser>();
        _outputWriter = new Mock<IOutputWriter>();

        _codeFile1 = new CodeFileProjectItem("", null, null);
        _codeFile2 = new CodeFileProjectItem("", null, null);
        _allCodeFiles = new[] { _codeFile1, _codeFile2 };
        _codeFile1ParsingContext = new Mock<ICodeFileParsingContext>();
        _codeFile2ParsingContext = new Mock<ICodeFileParsingContext>();
        _codeFile1ParsingContext.SetupGet(x => x.CodeFile).Returns(_codeFile1);
        _codeFile1ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(new ParseResult());
        _codeFile1ParsingContext.SetupGet(x => x.FunctionParserResult).Returns(new ParseResult());
        _codeFile2ParsingContext.SetupGet(x => x.CodeFile).Returns(_codeFile2);
        _codeFile2ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(new ParseResult());
        _codeFile2ParsingContext.SetupGet(x => x.FunctionParserResult).Returns(new ParseResult());
        _contextFactory.Setup(x => x.MakeContext(_allCodeFiles, _codeFile1)).Returns(_codeFile1ParsingContext.Object);
        _contextFactory.Setup(x => x.MakeContext(_allCodeFiles, _codeFile2)).Returns(_codeFile2ParsingContext.Object);
        _orderLogic.Setup(x => x.ByImports(_allCodeFiles)).Returns(_allCodeFiles);

        _builder = new CodeModelBuilder(_contextFactory.Object, _orderLogic.Object, _codeModelUpdateRaiser.Object, _outputWriter.Object);
    }

    [Test]
    public void BuildCodeModelsFor_Correct()
    {
        var result = _builder.BuildFor(_allCodeFiles);

        Assert.False(result.HasErrors);
        Assert.NotNull(_codeFile1.CodeModel);
        Assert.NotNull(_codeFile2.CodeModel);
        _codeFile1ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _orderLogic.Verify(x => x.ByImports(_allCodeFiles), Times.Once());
        _codeFile1ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile1), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile2), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(It.IsAny<CodeFileProjectItem>(), It.IsAny<ParserMessage>()), Times.Never());
        _outputWriter.Verify(x => x.WriteError(It.IsAny<CodeFileProjectItem>(), It.IsAny<ParserMessage>()), Times.Never());
    }

    [Test]
    public void BuildCodeModelsFor_Warnings()
    {
        var parseResult1 = new ParseResult();
        parseResult1.AddWarning(new(ParserMessageType.ConstNoValue, Token.Default));
        parseResult1.AddWarning(new(ParserMessageType.ConstIncorrectValue, Token.Default));
        _codeFile1ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult1);
        var parseResult2 = new ParseResult();
        parseResult2.AddWarning(new(ParserMessageType.ConstNoValue, Token.Default));
        parseResult2.AddWarning(new(ParserMessageType.ConstIncorrectValue, Token.Default));
        _codeFile2ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult2);

        var result = _builder.BuildFor(_allCodeFiles);

        Assert.IsFalse(result.HasErrors);
        Assert.NotNull(_codeFile1.CodeModel);
        Assert.NotNull(_codeFile2.CodeModel);
        _codeFile1ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _orderLogic.Verify(x => x.ByImports(_allCodeFiles), Times.Once());
        _codeFile1ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile1), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile2), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile1, parseResult1.Warnings.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile1, parseResult1.Warnings.Last()), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile2, parseResult2.Warnings.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile2, parseResult2.Warnings.Last()), Times.Once());
    }

    [Test]
    public void BuildCodeModelsFor_WarningsAndErrors()
    {
        var parseResult1 = new ParseResult();
        parseResult1.AddWarning(new(ParserMessageType.ConstNoValue, Token.Default));
        _codeFile1ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult1);
        var parseResult2 = new ParseResult();
        parseResult2.AddError(new(ParserMessageType.ConstIncorrectValue, Token.Default));
        parseResult2.AddError(new(ParserMessageType.FunctionNameExist, Token.Default));
        parseResult2.AddWarning(new(ParserMessageType.ConstNoValue, Token.Default));
        parseResult2.AddWarning(new(ParserMessageType.UnexpectedImport, Token.Default));
        _codeFile2ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult2);

        var result = _builder.BuildFor(_allCodeFiles);

        Assert.IsTrue(result.HasErrors);
        Assert.NotNull(_codeFile1.CodeModel);
        Assert.NotNull(_codeFile2.CodeModel);
        _codeFile1ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _orderLogic.Verify(x => x.ByImports(_allCodeFiles), Times.Once());
        _codeFile1ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseFunctions(), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile1), Times.Once());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile2), Times.Never());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile1, parseResult1.Warnings.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteError(_codeFile2, parseResult2.Errors.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteError(_codeFile2, parseResult2.Errors.Last()), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile2, parseResult2.Warnings.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile2, parseResult2.Warnings.Last()), Times.Once());
    }

    [Test]
    public void AssertRaiseUpdateModelForAllCodeFiles()
    {
        var result = _builder.BuildFor(_allCodeFiles);

        _codeModelUpdateRaiser.Verify(x => x.StoreOldCodeModels(_allCodeFiles), Times.Once());
        _codeModelUpdateRaiser.Verify(x => x.RaiseUpdateCodeModelWithDiff(), Times.Once());
    }
}
