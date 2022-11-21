using Luna.Output;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("print", "text")]
internal class Print : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        if (RuntimeEnvironment.StandartOutput != null)
        {
            var value = arguments.GetValueOrError<IRuntimeValue>(0);
            var text = value.ToString();
            if (value is StringRuntimeValue)
            {
                text = text.Substring(1, text.Length - 2);
            }
            var message = new OutputMessage(new[]
            {
                new OutputMessageItem(text, OutputMessageKind.Text)
            });
            RuntimeEnvironment.StandartOutput.SendMessage(message);
        }

        return VoidRuntimeValue.Instance;
    }
}
