using System.Collections.Generic;
using System.IO;
using Luna.Infrastructure;

namespace Luna.IDE.Infrastructure
{
    class FileSystem : IFileSystem
    {
        public IEnumerable<string> GetFiles(string path, string filter)
        {
            return Directory.GetFiles(path, filter);
        }

        public IEnumerable<string> GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }
    }
}
