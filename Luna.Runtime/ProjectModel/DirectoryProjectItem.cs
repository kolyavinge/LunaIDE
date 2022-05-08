using System.Collections.Generic;
using System.IO;

namespace Luna.ProjectModel
{
    public class DirectoryProjectItem : ProjectItem
    {
        private readonly List<ProjectItem> _children;

        public IReadOnlyCollection<ProjectItem> Children => _children;

        public IReadOnlyCollection<ProjectItem> AllChildren
        {
            get
            {
                var result = new List<ProjectItem> { this };
                foreach (var child in _children)
                {
                    if (child is DirectoryProjectItem directory) result.AddRange(directory.AllChildren);
                    else result.Add(child);
                }

                return result;
            }
        }

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
