using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Luna.ProjectModel
{
    public class DirectoryProjectItem : ProjectItem
    {
        private readonly List<ProjectItem> _children;

        public IReadOnlyCollection<ProjectItem> Children => _children;

        internal DirectoryProjectItem(string fullPath, ProjectItem? parent) : base(Path.GetFileName(fullPath), fullPath, parent)
        {
            _children = new List<ProjectItem>();
        }

        internal void AddChild(ProjectItem child)
        {
            _children.Add(child);
        }
    }
}
