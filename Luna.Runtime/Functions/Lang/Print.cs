using Luna.Collections;
using Luna.Output;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("print", "text")]
internal class Print : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        if (RuntimeEnvironment.StandartOutput != null)
        {
            var value = GetValueOrError<IRuntimeValue>(argumentValues, 0);
            var text = value.ToString() ?? "";
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
