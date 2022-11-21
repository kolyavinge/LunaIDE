﻿using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("lse", "x y")]
internal class Lse : EmbeddedFunction
{
    protected override IRuntimeValue InnerGetValue(EmbeddedFunctionArguments arguments)
    {
        var x = arguments.GetValueOrError<NumericRuntimeValue>(0);
        var y = arguments.GetValueOrError<NumericRuntimeValue>(1);

        return new BooleanRuntimeValue(x.FloatValue <= y.FloatValue);
    }
}
