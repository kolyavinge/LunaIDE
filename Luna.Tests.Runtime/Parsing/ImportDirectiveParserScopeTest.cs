using System;
using System.Collections.Generic;
using System.Text;
using Luna.Parsing;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.Parsing
{
    internal class ImportDirectiveParserScopeTest
    {
        private CodeFileProjectItem _codeFile1, _codeFile2;
        private ImportDirectiveParserScope _scope;

        [SetUp]
        public void Setup()
        {
            var directory = new DirectoryProjectItem("", null);
            _codeFile1 = new CodeFileProjectItem("file 1", directory, null);
            _codeFile2 = new CodeFileProjectItem("file 2", directory, null);
            _scope = new ImportDirectiveParserScope(new[] { _codeFile1, _codeFile2 });
        }

        [Test]
        public void GetCodeFileByPath()
        {
            Assert.AreEqual(_codeFile1, _scope.GetCodeFileByPath("file 1"));
            Assert.AreEqual(_codeFile2, _scope.GetCodeFileByPath("file 2"));
            Assert.AreEqual(null, _scope.GetCodeFileByPath("file"));
        }
    }
}
