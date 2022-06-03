using System;
using System.Linq;
using Luna.Collections;
using Luna.Utils;

namespace Luna.Runtime;

internal class FunctionRuntimeValue : IRuntimeValue
{
    private readonly IRuntimeScope _scope;

    public string Name { get; }

    public ReadonlyArray<IRuntimeValue>? AlreadyPassedArguments { get; set; }

    public FunctionRuntimeValue(string name, IRuntimeScope scope)
    {
        Name = name;
        _scope = scope;
    }

    public IRuntimeValue GetValue(ReadonlyArray<IRuntimeValue> argumentValues)
    {
        if (AlreadyPassedArguments != null)
        {
            argumentValues = AlreadyPassedArguments.Union(argumentValues).ToReadonlyArray();
        }
        var argumentNames = _scope.GetFunctionArgumentNames(Name);
        _scope.PushFunctionArguments();
        Enumerable.Range(0, Math.Min(argumentNames.Length, argumentValues.Count)).Each(index => _scope.AddFunctionArgument(argumentNames[index], argumentValues[index]));
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
            result = _scope.GetDeclaredFunctionValue(Name, argumentValues);
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
