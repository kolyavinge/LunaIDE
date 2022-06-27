﻿using Luna.Collections;
using Luna.Runtime;

namespace Luna.Functions.Math;

[EmbeddedFunctionDeclaration("ls", "x y")]
internal class Ls : EmbeddedFunction
{
    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var x = GetValueOrError<NumericRuntimeValue>(argumentValues, 0);
        var y = GetValueOrError<NumericRuntimeValue>(argumentValues, 1);

        return new BooleanRuntimeValue(x.FloatValue < y.FloatValue);
    }
}
