using System.Collections.Generic;
using Luna.Collections;
using Luna.Runtime;
using Moq;

namespace Luna.Tests.Tools;

internal class BaseFunctionRuntimeValueTest
{
    protected FunctionRuntimeValue _function;
    protected Mock<IRuntimeScope> _scope;
    protected Mock<IRuntimeExceptionHandler> _runtimeExceptionHandler;

    protected void Init()
    {
        _scope = new Mock<IRuntimeScope>();
        _runtimeExceptionHandler = new Mock<IRuntimeExceptionHandler>();
        RuntimeEnvironment.ExceptionHandler = _runtimeExceptionHandler.Object;
    }

    protected IRuntimeValue Eval(string funcName, IEnumerable<IRuntimeValue> arguments, ReadonlyArray<IRuntimeValue> alreadyPassedArguments = null)
    {
        _function = new FunctionRuntimeValue(funcName, _scope.Object) { AlreadyPassedArguments = alreadyPassedArguments };
        return _function.GetValue(new ReadonlyArray<IRuntimeValue>(arguments));
    }
}
