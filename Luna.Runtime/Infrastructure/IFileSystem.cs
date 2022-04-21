using System;
using System.Collections.Generic;
using System.Text;

namespace Luna.Infrastructure
{
    public interface IFileSystem
    {
        IEnumerable<string> GetFiles(string path, string filter);

        IEnumerable<string> GetDirectories(string path);
    }
}
