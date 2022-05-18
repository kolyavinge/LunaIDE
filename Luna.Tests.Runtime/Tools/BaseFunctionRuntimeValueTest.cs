using System.Collections.Generic;
using Luna.Collections;
using Luna.Runtime;
using Moq;

namespace Luna.Tests.Tools;

internal class BaseFunctionRuntimeValueTest
{
    protected Mock<IRuntimeScope> _scope;

    protected IRuntimeValue Eval(string funcName, IEnumerable<IRuntimeValue> arguments, ReadonlyArray<IRuntimeValue> alreadyPassedArguments = null)
    {
        var funcValue = new FunctionRuntimeValue(funcName, _scope.Object) { AlreadyPassedArguments = alreadyPassedArguments };
        return funcValue.GetValue(new ReadonlyArray<IRuntimeValue>(arguments));
    }
}
