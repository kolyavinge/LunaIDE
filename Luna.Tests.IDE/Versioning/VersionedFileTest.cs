using System.Text;
using Luna.IDE.Versioning;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.IDE.Versioning;

internal class VersionedFileTest
{
    private Mock<VersionControl.Core.IVersionControlRepository> _versionControlRepository;
    private VersionControl.Core.VersionedFile _versionedRepositoryFile;
    private VersionedFile _versionedFile;

    [SetUp]
    public void Setup()
    {
        _versionControlRepository = new Mock<VersionControl.Core.IVersionControlRepository>();
        _versionedRepositoryFile = new VersionControl.Core.VersionedFile(1, "", "", 1, VersionControl.Core.FileActionKind.Add);
        _versionedFile = new VersionedFile(_versionControlRepository.Object, _versionedRepositoryFile);
    }

    [Test]
    public void LastCommitContent()
    {
        _versionControlRepository.Setup(x => x.GetActualFileContent(_versionedRepositoryFile)).Returns(Encoding.UTF8.GetBytes("content"));
        Assert.That(_versionedFile.LastCommitContent, Is.EqualTo("content"));
    }

    [Test]
    public void LastCommitContent_Null()
    {
        _versionControlRepository.Setup(x => x.GetActualFileContent(_versionedRepositoryFile)).Returns((byte[])null);
        Assert.Null(_versionedFile.LastCommitContent);
    }
}
