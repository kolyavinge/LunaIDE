using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Luna.Infrastructure;
using Luna.ProjectModel;
using Moq;
using NUnit.Framework;

namespace Luna.Tests.ProjectModel
{
    internal class DirectoryProjectItemTest
    {
        private Mock<IFileSystem> _fileSystem;

        [SetUp]
        public void Setup()
        {
            _fileSystem = new Mock<IFileSystem>();
        }

        [Test]
        public void AllChildren()
        {
            var root = new DirectoryProjectItem(@"c:\path\project", null);

            var directory1 = new DirectoryProjectItem(@"c:\path\project\directory1", root);
            root.AddChild(directory1);

            var directory2 = new DirectoryProjectItem(@"c:\path\project\directory1\directory2", directory1);
            directory1.AddChild(directory2);

            var file = new CodeFileProjectItem(@"c:\path\project\directory1\directory2\codeFile", directory2, _fileSystem.Object);
            directory2.AddChild(file);

            var result = root.AllChildren.ToList();

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(root, result[0]);
            Assert.AreEqual(directory1, result[1]);
            Assert.AreEqual(directory2, result[2]);
            Assert.AreEqual(file, result[3]);
        }
    }
}
