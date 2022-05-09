using System.Linq;
using Luna.Output;
using Luna.Parsing;
using Luna.ProjectModel;

namespace Luna.Runtime
{
    public interface IInterpreter
    {
        void Run(Project project, IRuntimeOutput output);
    }

    public class Interpreter : IInterpreter
    {
        public void Run(Project project, IRuntimeOutput output)
        {
            var codeFiles = project.Root.AllChildren.OfType<CodeFileProjectItem>().ToList();
            var outputWriter = new OutputWriter(output);
            var codeModelBuilder = new CodeModelBuilder(outputWriter);
            var result = codeModelBuilder.BuildCodeModelsFor(codeFiles);
            if (result.HasErrors)
            {
                outputWriter.ProgramStopped();
                return;
            }
        }
    }
}
