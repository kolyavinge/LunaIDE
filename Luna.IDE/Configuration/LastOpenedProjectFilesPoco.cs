using System.Collections.Generic;
using System.Linq;

namespace Luna.IDE.Configuration;

internal class LastOpenedProjectFilesPoco : IEquatable<LastOpenedProjectFilesPoco?>, IConfigStoragePoco
{
    public string ProjectFullPath { get; set; } = "";

    public List<string> FilesRelativePathes { get; set; } = new();

    public override bool Equals(object? obj)
    {
        return Equals(obj as LastOpenedProjectFilesPoco);
    }

    public bool Equals(LastOpenedProjectFilesPoco? other)
    {
        return other is not null &&
               ProjectFullPath == other.ProjectFullPath &&
               FilesRelativePathes.SequenceEqual(other.FilesRelativePathes);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ProjectFullPath);
        foreach (var item in FilesRelativePathes)
        {
            hash.Add(item);
        }

        return hash.ToHashCode();
    }
}
