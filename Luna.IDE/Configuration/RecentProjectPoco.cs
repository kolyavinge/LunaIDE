namespace Luna.IDE.Configuration;

internal class RecentProjectPoco : IEquatable<RecentProjectPoco?>, IConfigStoragePoco
{
    public int Id { get; set; }

    public string ProjectFullPath { get; set; } = "";

    public DateTime LastAccess { get; set; }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RecentProjectPoco);
    }

    public bool Equals(RecentProjectPoco? other)
    {
        return other is not null &&
               Id == other.Id &&
               ProjectFullPath == other.ProjectFullPath &&
               LastAccess == other.LastAccess;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, ProjectFullPath, LastAccess);
    }
}
