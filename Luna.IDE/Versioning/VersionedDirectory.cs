using VersionControl.Core;

namespace Luna.IDE.Versioning;

public class VersionedDirectory : AbstractDirectory<VersionedDirectory, VersionedFile>
{
    internal VersionedDirectory(string name)
        : base(name, dir => new VersionedDirectory(dir), file => file.RelativePath)
    {
    }
}
