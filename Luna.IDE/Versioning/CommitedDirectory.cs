namespace Luna.IDE.Versioning;

public class CommitedDirectory : AbstractDirectory<CommitedDirectory, CommitedFile>
{
    internal CommitedDirectory(string name)
        : base(name, dir => new CommitedDirectory(dir), file => file.RelativePath)
    {
    }
}
