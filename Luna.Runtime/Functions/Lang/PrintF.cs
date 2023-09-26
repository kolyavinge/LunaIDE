using System.Linq;
using Luna.Output;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("printf", "format params")]
internal class PrintF : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        if (RuntimeEnvironment.StandartOutput is not null)
        {
            var format = arguments.GetValueOrError<StringRuntimeValue>(0).ToString();
            var param = arguments.GetValueOrError<ListRuntimeValue>(1).Select(x => x.ToString()).ToArray();
            var text = String.Format(format, param);
            text = text.Substring(1, text.Length - 2);
            var message = new OutputMessage(new[]
            {
                new OutputMessageItem(text, OutputMessageKind.Text)
            });
            RuntimeEnvironment.StandartOutput.SendMessage(message);
        }

        return VoidRuntimeValue.Instance;
    }
}
