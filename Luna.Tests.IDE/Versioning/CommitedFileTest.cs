using System.Text;
using Luna.IDE.Versioning;
using Moq;
using NUnit.Framework;
using VersionControl.Core;

namespace Luna.Tests.IDE.Versioning;

internal class CommitedFileTest
{
    private Mock<IVersionControlRepository> _versionControlRepository;
    private CommitDetail _detail;
    private CommitedFile _commitedFile;

    [SetUp]
    public void Setup()
    {
        _versionControlRepository = new Mock<IVersionControlRepository>();
        _detail = new CommitDetail(1, FileActionKind.Add, 1, "");
        _commitedFile = new CommitedFile(_versionControlRepository.Object, _detail);
    }

    [Test]
    public void Content()
    {
        _versionControlRepository.Setup(x => x.GetFileContent(_detail)).Returns(Encoding.UTF8.GetBytes("content"));
        Assert.That(_commitedFile.Content, Is.EqualTo("content"));
    }

    [Test]
    public void BeforeContent()
    {
        _versionControlRepository.Setup(x => x.GetFileContentBefore(_detail)).Returns(Encoding.UTF8.GetBytes("before content"));
        Assert.That(_commitedFile.BeforeContent, Is.EqualTo("before content"));
    }

    [Test]
    public void BeforeContent_Null()
    {
        _versionControlRepository.Setup(x => x.GetFileContentBefore(_detail)).Returns((byte[])null);
        Assert.Null(_commitedFile.BeforeContent);
    }
}
