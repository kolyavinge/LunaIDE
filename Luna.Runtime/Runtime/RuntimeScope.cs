using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;
using Luna.Collections;
using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal class RuntimeScope : IRuntimeScope
{
    public static IRuntimeScope FromCodeModel(
        CodeModel codeModel,
        IValueElementEvaluator evaluator,
        IEmbeddedFunctionsCollection embeddedFunctions,
        CallStack callStack)
    {
        var constants = codeModel.Constants.ToList();
        var functions = codeModel.Functions.ToList();
        foreach (var import in codeModel.Imports)
        {
            var importCodeModel = import.CodeFile.CodeModel;
            constants.AddRange(importCodeModel.Constants);
            functions.AddRange(importCodeModel.Functions);
        }

        return new RuntimeScope(evaluator, embeddedFunctions, functions, constants, callStack);
    }

    private int _lambdaNameIncrement = 0;
    private readonly IValueElementEvaluator _evaluator;
    private readonly IEmbeddedFunctionsCollection _embeddedFunctions;
    private readonly Dictionary<string, FunctionDeclaration> _declaredFunctions;
    private readonly Dictionary<string, ScopeLambdaDeclaration> _declaredLambdas;
    private readonly Dictionary<string, ConstantDeclaration> _constantDeclarations;
    private readonly Dictionary<string, VariableRuntimeValue> _variables;
    private readonly CallStack _callStack;
    private readonly Stack<Dictionary<string, IRuntimeValue>> _argumentStack;

    public RuntimeScope(
        IValueElementEvaluator evaluator,
        IEmbeddedFunctionsCollection embeddedFunctions,
        IEnumerable<FunctionDeclaration> declaredFunctions,
        IEnumerable<ConstantDeclaration> constantDeclarations,
        CallStack callStack)
    {
        _lambdaNameIncrement = 0;
        _evaluator = evaluator;
        _embeddedFunctions = embeddedFunctions;
        _declaredFunctions = declaredFunctions.ToDictionary(x => x.Name, v => v);
        _declaredLambdas = new();
        _constantDeclarations = constantDeclarations.ToDictionary(x => x.Name, v => v);
        _variables = new();
        _callStack = callStack;
        _argumentStack = new();
    }

    public ValueElement GetConstantValue(string constantName)
    {
        return _constantDeclarations[constantName].Value;
    }

    public FunctionDeclaration GetFunctionDeclaration(string functionName)
    {
        return _declaredFunctions[functionName];
    }

    public bool IsDeclaredOrEmbeddedFunction(string functionName)
    {
        return _declaredFunctions.ContainsKey(functionName) || _embeddedFunctions.Contains(functionName);
    }

    public VariableRuntimeValue GetVariableOrCreateNew(string variableName)
    {
        if (_variables.ContainsKey(variableName))
        {
            return _variables[variableName];
        }
        else
        {
            var variable = new VariableRuntimeValue();
            _variables.Add(variableName, variable);
            return variable;
        }
    }

    public string[] GetFunctionArgumentNames(string functionName)
    {
        if (_embeddedFunctions.Contains(functionName))
        {
            return _embeddedFunctions.GetByName(functionName).Arguments;
        }
        else if (_declaredFunctions.ContainsKey(functionName))
        {
            return _declaredFunctions[functionName].Arguments.Select(x => x.Name).ToArray();
        }
        else
        {
            return _declaredLambdas[functionName].Arguments.Select(x => x.Name).ToArray();
        }
    }

    public IRuntimeValue GetFunctionArgumentValue(string argumentName)
    {
        return _argumentStack.Peek()[argumentName];
    }

    public void AddFunctionArgument(string argumentName, IRuntimeValue argumentValue)
    {
        _argumentStack.Peek().Add(argumentName, argumentValue);
    }

    public void PushCallStack(IFunctionRuntimeValue function)
    {
        _callStack.Push(function);
        _argumentStack.Push(new());
    }

    public void PopCallStack()
    {
        _callStack.Pop();
        _argumentStack.Pop();
    }

    public IRuntimeValue GetDeclaredFunctionValue(string functionName)
    {
        IRuntimeValue result = VoidRuntimeValue.Instance;
        var body = GetFunctionBody(functionName);
        foreach (var bodyItem in body)
        {
            result = _evaluator.Eval(this, bodyItem).GetValue();
        }

        return result;
    }

    private FunctionBody GetFunctionBody(string functionName)
    {
        return _declaredFunctions.ContainsKey(functionName)
            ? _declaredFunctions[functionName].Body
            : _declaredLambdas[functionName].Body;
    }

    public bool IsEmbeddedFunction(string functionName)
    {
        return _embeddedFunctions.Contains(functionName);
    }

    public IRuntimeValue GetEmbeddedFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var func = _embeddedFunctions.GetByName(functionName);
        return func.GetValue(argumentValues);
    }

    public AddLambdaResult AddLambda(LambdaValueElement lambdaElement)
    {
        var name = $"#lambda_{_lambdaNameIncrement}";
        _lambdaNameIncrement++;
        var stackArguments = _argumentStack.Peek().Keys.Select(x => new FunctionArgument(x)).ToList();
        var alreadyPassedArguments = stackArguments.Select(x => GetFunctionArgumentValue(x.Name)).ToReadonlyArray();
        var arguments = stackArguments.Union(lambdaElement.Arguments).ToReadonlyArray();
        _declaredLambdas.Add(name, new(arguments, lambdaElement.Body));

        return new(name, alreadyPassedArguments);
    }

    class ScopeLambdaDeclaration
    {
        public ReadonlyArray<FunctionArgument> Arguments { get; }
        public FunctionBody Body { get; }
        public ScopeLambdaDeclaration(ReadonlyArray<FunctionArgument> arguments, FunctionBody body)
        {
            Arguments = arguments;
            Body = body;
        }
    }
}
