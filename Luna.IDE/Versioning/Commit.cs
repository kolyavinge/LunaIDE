using System.Linq;

namespace Luna.IDE.Versioning;

public interface ICommit
{
    long Id { get; }
    string Author { get; }
    string Comment { get; }
    DateTime Created { get; }
    CommitedDirectory Details { get; }
}

internal class Commit : ICommit
{
    private readonly string _projectName;
    private readonly VersionControl.Core.IVersionControlRepository _versionControlRepository;
    private readonly VersionControl.Core.Commit _commit;

    public long Id => _commit.Id;
    public string Author => _commit.Author;
    public string Comment => _commit.Comment;
    public DateTime Created => _commit.CreatedUtc.ToLocalTime();

    public CommitedDirectory Details
    {
        get
        {
            var commitDetails = _versionControlRepository.GetCommitDetails(_commit);
            var commitedFiles = commitDetails.Select(detail => new CommitedFile(_versionControlRepository, detail)).ToList();
            var commitedDirectory = new CommitedDirectory(_projectName);
            commitedDirectory.AddFiles(commitedFiles);

            return commitedDirectory;
        }
    }

    public Commit(
        string projectName,
        VersionControl.Core.IVersionControlRepository versionControlRepository,
        VersionControl.Core.Commit commit)
    {
        _projectName = projectName;
        _versionControlRepository = versionControlRepository;
        _commit = commit;
    }
}
