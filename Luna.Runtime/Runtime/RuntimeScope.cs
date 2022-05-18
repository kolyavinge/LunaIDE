﻿using System;
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
            var importCodeModel = import.CodeFile.CodeModel!;
            constants.AddRange(importCodeModel.Constants);
            functions.AddRange(importCodeModel.Functions);
        }

        return new RuntimeScope(evaluator, embeddedFunctions, functions, constants);
    }

    private readonly IValueElementEvaluator _evaluator;
    private readonly IEmbeddedFunctionsCollection _embeddedFunctions;
    private readonly Dictionary<string, ScopeFunctionDeclaration> _declaredFunctions;
    private readonly Dictionary<string, ConstDeclaration> _constDeclarations;

    private readonly Stack<Dictionary<string, IRuntimeValue>> _argumentStack;

    public RuntimeScope(
        IValueElementEvaluator evaluator,
        IEmbeddedFunctionsCollection embeddedFunctions,
        IEnumerable<FunctionDeclaration> declaredFunctions,
        IEnumerable<ConstDeclaration> constDeclarations)
    {
        _evaluator = evaluator;
        _embeddedFunctions = embeddedFunctions;
        _declaredFunctions = declaredFunctions.Select(x => new ScopeFunctionDeclaration(x.Name, x.Arguments, x.Body)).ToDictionary(x => x.Name, v => v);
        _constDeclarations = constDeclarations.ToDictionary(x => x.Name, v => v);
        _argumentStack = new();
        _argumentStack.Push(new());
    }

    public ValueElement GetConstantValue(string constantName)
    {
        return _constDeclarations[constantName].Value;
    }

    public bool ArgumentCalledAsFunction(string argumentName)
    {
        var args = _argumentStack.Peek();
        return args.ContainsKey(argumentName) && args[argumentName] is FunctionRuntimeValue;
    }

    public bool IsDeclaredFunction(string functionName)
    {
        return _declaredFunctions.ContainsKey(functionName);
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

    public void RemoveFunctionArgument(string argumentName)
    {
        _argumentStack.Peek().Remove(argumentName);
    }

    public IRuntimeValue GetDeclaredFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues)
    {
        IRuntimeValue? result = null;
        var declaration = _declaredFunctions[functionName];
        foreach (var item in declaration.Body)
        {
            result = _evaluator.Eval(this, item);
        }

        return result!;
    }

    public bool IsEmbeddedFunction(string functionName)
    {
        return _embeddedFunctions.Contains(functionName);
    }

    public IRuntimeValue GetEmbeddedFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues)
    {
        return _embeddedFunctions.GetByName(functionName).GetValue(argumentValues);
    }

    public string AddLambda(LambdaValueElement lambdaElement)
    {
        var name = $"lambda_{DateTime.UtcNow.ToBinary()}";
        _declaredFunctions.Add(name, new ScopeFunctionDeclaration(name, lambdaElement.Arguments, lambdaElement.Body));
        return name;
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
