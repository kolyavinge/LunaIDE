using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Common;
using NUnit.Framework;

namespace Luna.Tests.IDE.Common;

public class TreeItemTest
{
    class TestTreeItem : TreeItem
    {
        private readonly List<TestTreeItem> _children = new();

        public TestTreeItem(TreeItem parent) : base(parent, "", null) { }

        protected override IEnumerable<TreeItem> GetChildren() => _children;

        public void AddChild(TestTreeItem child) => _children.Add(child);
    }

    private TestTreeItem _root;

    [SetUp]
    public void Setup()
    {
        _root = new TestTreeItem(null);
        var directory = new TestTreeItem(_root);
        _root.AddChild(directory);
        var file = new TestTreeItem(directory);
        directory.AddChild(file);
    }

    [Test]
    public void Depth()
    {
        Assert.That(_root.Depth, Is.EqualTo(0));
        Assert.That(_root.Children.First().Depth, Is.EqualTo(0));
        Assert.That(_root.Children.First().Children.First().Depth, Is.EqualTo(1));
    }

    [Test]
    public void CollapseWithSelectedChildren()
    {
        _root.IsExpanded = true;
        _root.Children.First().IsSelected = true;
        _root.IsExpanded = false;
        Assert.True(_root.IsSelected);
    }
}
