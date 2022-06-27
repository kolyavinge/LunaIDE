using System;
using System.Linq;
using Luna.Collections;
using Luna.Utils;

namespace Luna.Runtime;

internal class FunctionRuntimeValue : RuntimeValue
{
    private readonly IRuntimeScope _scope;

    public string Name { get; }

    public ReadonlyArray<IRuntimeValue>? AlreadyPassedArguments { get; set; }

    public FunctionRuntimeValue(string name, IRuntimeScope scope)
    {
        Name = name;
        _scope = scope;
    }

    public override IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue>? argumentValues = null)
    {
        argumentValues ??= new ReadonlyArray<IRuntimeValue>();
        if (AlreadyPassedArguments != null)
        {
            argumentValues = AlreadyPassedArguments.Concat(argumentValues).ToReadonlyArray();
        }
        var argumentNames = _scope.GetFunctionArgumentNames(Name);
        _scope.PushFunctionArguments();
        Enumerable.Range(0, Math.Min(argumentNames.Length, argumentValues.Count)).Each(i => _scope.AddFunctionArgument(argumentNames[i], argumentValues[i]));
        IRuntimeValue result;
        if (argumentValues.Count < argumentNames.Length)
        {
            result = new FunctionRuntimeValue(Name, _scope) { AlreadyPassedArguments = argumentValues };
        }
        else if (_scope.IsEmbeddedFunction(Name))
        {
            result = _scope.GetEmbeddedFunctionValue(Name, argumentValues);
        }
        else
        {
            result = _scope.GetDeclaredFunctionValue(Name);
        }
        _scope.PopFunctionArguments();
        if (argumentValues.Count - argumentNames.Length > 0)
        {
            if (result is FunctionRuntimeValue resultFunction)
            {
                result = resultFunction.GetValue(argumentValues.Skip(argumentNames.Length).ToReadonlyArray());
            }
            else throw RuntimeException.ToManyArguments(Name);
        }

        return result;
    }

    public override string ToString()
    {
        return Name;
    }
}
