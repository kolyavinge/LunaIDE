using System.Text;
using VersionControl.Core;

namespace Luna.IDE.Versioning;

public class CommitedFile
{
    private readonly IVersionControlRepository _versionControlRepository;
    private readonly CommitDetail _detail;

    public string RelativePath => _detail.RelativePath;

    public FileActionKind ActionKind => _detail.FileActionKind;

    public string Content
    {
        get
        {
            var bytes = _versionControlRepository.GetFileContent(_detail);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    public string? BeforeContent
    {
        get
        {
            var bytes = _versionControlRepository.GetFileContentBefore(_detail);
            return bytes is not null ? Encoding.UTF8.GetString(bytes) : null;
        }
    }

    internal CommitedFile(IVersionControlRepository versionControlRepository, CommitDetail detail)
    {
        _versionControlRepository = versionControlRepository;
        _detail = detail;
    }
}
