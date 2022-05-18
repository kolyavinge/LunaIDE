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
        IRuntimeValue result;
        if (_scope.ArgumentCalledAsFunction(Name))
        {
            var value = _scope.GetFunctionArgumentValue(Name);
            if (value is FunctionRuntimeValue funcArgValue) result = funcArgValue.GetValue(argumentValues);
            else throw RuntimeException.IsNotFunction(this);
        }
        else
        {
            var argumentNames = _scope.GetFunctionArgumentNames(Name);
            if (argumentValues.Count > argumentNames.Length) throw RuntimeException.ToManyArguments(this);
            argumentValues.Each((argumentValue, index) => _scope.AddFunctionArgument(argumentNames[index], argumentValue));
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
            argumentNames.Each(_scope.RemoveFunctionArgument);
        }

        return result;
    }

    public override string ToString()
    {
        return Name;
    }
}
