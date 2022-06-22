using System.Collections.Generic;
using System.Linq;
using Luna.Collections;
using Luna.Functions;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal class RuntimeScope : IRuntimeScope
{
    public static IRuntimeScope FromCodeModel(
        CodeModel codeModel,
        IValueElementEvaluator evaluator,
        IEmbeddedFunctionsCollection embeddedFunctions)
    {
        var constants = codeModel.Constants.ToList();
        var functions = codeModel.Functions.ToList();
        foreach (var import in codeModel.Imports)
        {
            var importCodeModel = import.CodeFile.CodeModel;
            constants.AddRange(importCodeModel.Constants);
            functions.AddRange(importCodeModel.Functions);
        }

        return new RuntimeScope(evaluator, embeddedFunctions, functions, constants);
    }

    private readonly IValueElementEvaluator _evaluator;
    private readonly IEmbeddedFunctionsCollection _embeddedFunctions;
    private readonly Dictionary<string, ScopeFunctionDeclaration> _declaredFunctions;
    private readonly Dictionary<string, ConstantDeclaration> _constantDeclarations;
    private readonly Dictionary<string, VariableRuntimeValue> _variables;

    private readonly Stack<Dictionary<string, IRuntimeValue>> _argumentStack;

    public RuntimeScope(
        IValueElementEvaluator evaluator,
        IEmbeddedFunctionsCollection embeddedFunctions,
        IEnumerable<FunctionDeclaration> declaredFunctions,
        IEnumerable<ConstantDeclaration> constantDeclarations)
    {
        _evaluator = evaluator;
        _embeddedFunctions = embeddedFunctions;
        _declaredFunctions = declaredFunctions.Select(x => new ScopeFunctionDeclaration(x.Name, x.Arguments, x.Body)).ToDictionary(x => x.Name, v => v);
        _constantDeclarations = constantDeclarations.ToDictionary(x => x.Name, v => v);
        _variables = new();
        _argumentStack = new();
        _argumentStack.Push(new());
    }

    public ValueElement GetConstantValue(string constantName)
    {
        return _constantDeclarations[constantName].Value;
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
        return _embeddedFunctions.Contains(functionName)
            ? _embeddedFunctions.GetByName(functionName).Arguments
            : _declaredFunctions[functionName].Arguments.Select(x => x.Name).ToArray();
    }

    public IRuntimeValue GetFunctionArgumentValue(string argumentName)
    {
        return _argumentStack.Peek()[argumentName];
    }

    public void PushFunctionArguments()
    {
        _argumentStack.Push(new());
    }

    public void PopFunctionArguments()
    {
        _argumentStack.Pop();
    }

    public void AddFunctionArgument(string argumentName, IRuntimeValue argumentValue)
    {
        _argumentStack.Peek().Add(argumentName, argumentValue);
    }

    public IRuntimeValue GetDeclaredFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues)
    {
        IRuntimeValue result = VoidRuntimeValue.Instance;
        var declaration = _declaredFunctions[functionName];
        foreach (var item in declaration.Body)
        {
            result = _evaluator.Eval(this, item).GetValue();
        }

        return result;
    }

    public bool IsEmbeddedFunction(string functionName)
    {
        return _embeddedFunctions.Contains(functionName);
    }

    public IRuntimeValue GetEmbeddedFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues)
    {
        var func = _embeddedFunctions.GetByName(functionName);
        func.SetArgumentValues(argumentValues);
        return func.GetValue();
    }

    private int _lambdaNameIncrement;
    public AddLambdaResult AddLambda(LambdaValueElement lambdaElement)
    {
        var name = $"lambda_{_lambdaNameIncrement}";
        _lambdaNameIncrement++;
        var currentArguments = _argumentStack.Peek().Keys.Select(x => new FunctionArgument(x)).ToList();
        var alreadyPassedArguments = currentArguments.Select(x => GetFunctionArgumentValue(x.Name)).ToReadonlyArray();
        var arguments = currentArguments.Union(lambdaElement.Arguments).ToReadonlyArray();
        _declaredFunctions.Add(name, new ScopeFunctionDeclaration(name, arguments, lambdaElement.Body));

        return new(name, alreadyPassedArguments);
    }

    class ScopeFunctionDeclaration
    {
        public string Name { get; }
        public ReadonlyArray<FunctionArgument> Arguments { get; }
        public FunctionBody Body { get; }

        public ScopeFunctionDeclaration(string name, ReadonlyArray<FunctionArgument> arguments, FunctionBody body)
        {
            Name = name;
            Arguments = arguments;
            Body = body;
        }
    }
}
