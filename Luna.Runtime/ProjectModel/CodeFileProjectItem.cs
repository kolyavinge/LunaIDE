using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Luna.Infrastructure;

namespace Luna.ProjectModel
{
    public class CodeFileProjectItem : FileProjectItem
    {
        internal CodeFileProjectItem(string fullPath, ProjectItem? parent, IFileSystem fileSystem) : base(fullPath, parent, fileSystem)
        {
        }
    }
}
