using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Luna.ProjectModel
{
    public class CodeFileProjectItem : ProjectItem
    {
        internal CodeFileProjectItem(string fullPath, ProjectItem? parent) : base(Path.GetFileName(fullPath), fullPath, parent)
        {
        }
    }
}
