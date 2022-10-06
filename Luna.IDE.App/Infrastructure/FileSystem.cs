using System.Collections.Generic;
using System.IO;
using Luna.Infrastructure;

namespace Luna.IDE.App.Infrastructure;

class FileSystem : IFileSystem
{
    public IEnumerable<string> GetFiles(string path, string filter)
    {
        return Directory.GetFiles(path, filter, new EnumerationOptions { AttributesToSkip = FileAttributes.Hidden });
    }

    public IEnumerable<string> GetDirectories(string path)
    {
        return Directory.GetDirectories(path, "*", new EnumerationOptions { AttributesToSkip = FileAttributes.Hidden });
    }

    public string ReadFileText(string fullPath)
    {
        return File.ReadAllText(fullPath);
    }

    public void SaveFileText(string fullPath, string text)
    {
        File.WriteAllText(fullPath, text);
    }
}
