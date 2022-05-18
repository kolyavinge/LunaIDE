using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.Parsing;

internal class CodeModelBuilderTest
{
    private CodeFileProjectItem _codeFile1, _codeFile2;
    private CodeFileProjectItem[] _allCodeFiles;
    private Mock<ICodeFileParsingContext> _codeFile1ParsingContext, _codeFile2ParsingContext;
    private Mock<ICodeFileParsingContextFactory> _contextFactory;
    private Mock<ICodeFileOrderLogic> _orderLogic;
    private Mock<IOutputWriter> _outputWriter;
    private CodeModelBuilder _builder;

    [SetUp]
    public void Setup()
    {
        _contextFactory = new Mock<ICodeFileParsingContextFactory>();
        _orderLogic = new Mock<ICodeFileOrderLogic>();
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

        _builder = new CodeModelBuilder(_contextFactory.Object, _orderLogic.Object, _outputWriter.Object);
    }

    [Test]
    public void BuildCodeModelsFor_Correct()
    {
        var result = _builder.BuildCodeModelsFor(_allCodeFiles);

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
        parseResult1.AddWarning(ParserMessageType.ConstNoValue, new Token());
        parseResult1.AddWarning(ParserMessageType.ConstIncorrectValue, new Token());
        _codeFile1ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult1);
        var parseResult2 = new ParseResult();
        parseResult2.AddWarning(ParserMessageType.ConstNoValue, new Token());
        parseResult2.AddWarning(ParserMessageType.ConstIncorrectValue, new Token());
        _codeFile2ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult2);

        var result = _builder.BuildCodeModelsFor(_allCodeFiles);

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
        parseResult1.SetError(ParserMessageType.FunctionNameExist, new Token());
        parseResult1.AddWarning(ParserMessageType.ConstNoValue, new Token());
        _codeFile1ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult1);
        var parseResult2 = new ParseResult();
        parseResult2.SetError(ParserMessageType.FunctionNameExist, new Token());
        parseResult2.AddWarning(ParserMessageType.ConstNoValue, new Token());
        _codeFile2ParsingContext.SetupGet(x => x.ImportDirectivesResult).Returns(parseResult2);

        var result = _builder.BuildCodeModelsFor(_allCodeFiles);

        Assert.IsTrue(result.HasErrors);
        Assert.NotNull(_codeFile1.CodeModel);
        Assert.NotNull(_codeFile2.CodeModel);
        _codeFile1ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _codeFile2ParsingContext.Verify(x => x.ParseImports(), Times.Once());
        _orderLogic.Verify(x => x.ByImports(_allCodeFiles), Times.Never());
        _codeFile1ParsingContext.Verify(x => x.ParseFunctions(), Times.Never());
        _codeFile2ParsingContext.Verify(x => x.ParseFunctions(), Times.Never());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile1), Times.Never());
        _outputWriter.Verify(x => x.SuccessfullyParsed(_codeFile2), Times.Never());
        _outputWriter.Verify(x => x.WriteError(_codeFile1, parseResult1.Error), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile1, parseResult1.Warnings.First()), Times.Once());
        _outputWriter.Verify(x => x.WriteError(_codeFile2, parseResult2.Error), Times.Once());
        _outputWriter.Verify(x => x.WriteWarning(_codeFile2, parseResult2.Warnings.First()), Times.Once());
    }
}
