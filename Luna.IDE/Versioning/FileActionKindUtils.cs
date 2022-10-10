using VersionControl.Core;

namespace Luna.IDE.Versioning;

public static class FileActionKindUtils
{
    public static string FileActionKindToString(FileActionKind actionKind)
    {
        return actionKind switch
        {
            FileActionKind.Add => "[add]",
            FileActionKind.Modify => "[edit]",
            FileActionKind.Replace => "[rename]",
            FileActionKind.ModifyAndReplace => "[edit, rename]",
            FileActionKind.Delete => "[del]",
            _ => throw new NotImplementedException()
        };
    }
}
