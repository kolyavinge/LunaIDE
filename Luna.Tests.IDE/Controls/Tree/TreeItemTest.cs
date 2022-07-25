using System.Collections.Generic;
using System.Linq;
using Luna.IDE.Controls.Tree;
using NUnit.Framework;

namespace Luna.Tests.IDE.Controls.Tree;

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
        Assert.AreEqual(0, _root.Depth);
        Assert.AreEqual(0, _root.Children.First().Depth);
        Assert.AreEqual(1, _root.Children.First().Children.First().Depth);
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
