using Luna.Functions;
using Luna.Runtime;

namespace Luna.Tests.Functions;

internal class EmbeddedFunctionTest
{
    [EmbeddedFunctionDeclaration("test", "x")]
    class TestEmbeddedFunction : EmbeddedFunction
    {
        protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments) => new BooleanRuntimeValue(true);
    }
}
