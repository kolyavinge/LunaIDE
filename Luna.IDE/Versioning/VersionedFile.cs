using System.Text;
using VersionControl.Core;

namespace Luna.IDE.Versioning;

public class VersionedFile : IEquatable<VersionedFile?>
{
    private readonly IVersionControlRepository _versionControlRepository;

    public VersionControl.Core.VersionedFile InnerVersionedFile { get; }
    public string RelativePath => InnerVersionedFile.RelativePath;
    public string FullPath => InnerVersionedFile.FullPath;
    public FileActionKind ActionKind => InnerVersionedFile.ActionKind;

    public VersionedFile(IVersionControlRepository versionControlRepository, VersionControl.Core.VersionedFile versionedFile)
    {
        _versionControlRepository = versionControlRepository;
        InnerVersionedFile = versionedFile;
    }

    public string? LastCommitContent
    {
        get
        {
            var bytes = _versionControlRepository.GetActualFileContent(InnerVersionedFile);
            return bytes is not null ? Encoding.UTF8.GetString(bytes) : null;
        }
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as VersionedFile);
    }

    public bool Equals(VersionedFile? other)
    {
        return other is not null &&
               InnerVersionedFile.Equals(other.InnerVersionedFile);
    }

    public override int GetHashCode()
    {
        return InnerVersionedFile.GetHashCode();
    }
}
