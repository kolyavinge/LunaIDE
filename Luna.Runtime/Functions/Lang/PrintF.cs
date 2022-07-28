using System;
using System.Linq;
using Luna.Collections;
using Luna.Output;
using Luna.Runtime;

namespace Luna.Functions.Lang;

[EmbeddedFunctionDeclaration("printf", "format params")]
internal class PrintF : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        if (RuntimeEnvironment.StandartOutput != null)
        {
            var format = GetValueOrError<StringRuntimeValue>(argumentValues, 0).ToString() ?? "";
            var param = GetValueOrError<ListRuntimeValue>(argumentValues, 1).Select(x => x.ToString()).ToArray();
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
