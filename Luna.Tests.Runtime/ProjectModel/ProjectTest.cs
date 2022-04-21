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
    public class ProjectTest
    {
        private Project _project;
        private Mock<IFileSystem> _fileSystem;

        [SetUp]
        public void Setup()
        {
            _fileSystem = new Mock<IFileSystem>();
            _fileSystem.Setup(x => x.GetDirectories(@"c:\path\project")).Returns(new[] { @"c:\path\project\directory" });
            _fileSystem.Setup(x => x.GetDirectories(@"c:\path\project\directory")).Returns(new string[0]);
            _fileSystem.Setup(x => x.GetFiles(@"c:\path\project", "*.luna")).Returns(new[] { @"c:\path\project\file" });
            _fileSystem.Setup(x => x.GetFiles(@"c:\path\project\directory", "*.luna")).Returns(new[] { @"c:\path\project\directory\file" });
        }

        [Test]
        public void Read()
        {
            _project = new Project(@"c:\path\project", _fileSystem.Object);

            Assert.AreEqual(null, _project.Root.Parent);
            Assert.AreEqual(@"c:\path\project", _project.Root.FullPath);
            Assert.AreEqual("project", _project.Root.Name);

            var rootChildren = _project.Root.Children.ToList();
            Assert.AreEqual(2, rootChildren.Count);

            Assert.AreEqual(_project.Root, rootChildren[0].Parent);
            Assert.AreEqual(@"c:\path\project\directory", rootChildren[0].FullPath);
            Assert.AreEqual("directory", rootChildren[0].Name);

            Assert.AreEqual(_project.Root, rootChildren[1].Parent);
            Assert.AreEqual(@"c:\path\project\file", rootChildren[1].FullPath);
            Assert.AreEqual("file", rootChildren[1].Name);

            var directoryChildren = ((DirectoryProjectItem)rootChildren.First()).Children.ToList();
            Assert.AreEqual(1, directoryChildren.Count);

            Assert.AreEqual(rootChildren[0], directoryChildren[0].Parent);
            Assert.AreEqual(@"c:\path\project\directory\file", directoryChildren[0].FullPath);
            Assert.AreEqual("file", directoryChildren[0].Name);
        }
    }
}
