﻿using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class EqTest : BaseFunctionTest<Eq>
{
    [Test]
    public void GetValue_IntegerAndFloat()
    {
        var result = GetValue<BooleanRuntimeValue>(new IntegerRuntimeValue(5), new FloatRuntimeValue(5.0));
        Assert.True(result.Value);

        result = GetValue<BooleanRuntimeValue>(new FloatRuntimeValue(5.0), new IntegerRuntimeValue(5));
        Assert.True(result.Value);
    }
}
