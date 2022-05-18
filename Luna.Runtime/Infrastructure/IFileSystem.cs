using System.Collections.Generic;

namespace Luna.Infrastructure;

public interface IFileSystem
{
    IEnumerable<string> GetFiles(string path, string filter);

    IEnumerable<string> GetDirectories(string path);

    string ReadFileText(string fullPath);

    void SaveFileText(string fullPath, string text);
}
