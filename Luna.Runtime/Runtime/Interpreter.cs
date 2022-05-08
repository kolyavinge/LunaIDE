using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.Runtime
{
    public class Interpreter
    {
        public static Interpreter MakeFor(Project project, IRuntimeOutput output) => new Interpreter(project, output);

        private readonly Project _project;
        private readonly IOutputWriter _outputWriter;
        private readonly CodeModelBuilder _codeModelBuilder;

        internal Interpreter(Project project, IRuntimeOutput output)
        {
            _project = project;
            _outputWriter = new OutputWriter(output);
            _codeModelBuilder = new CodeModelBuilder(_outputWriter);
        }

        public void Run()
        {
            var codeFiles = _project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
            _codeModelBuilder.BuildCodeModelsFor(codeFiles);
        }
    }
}
