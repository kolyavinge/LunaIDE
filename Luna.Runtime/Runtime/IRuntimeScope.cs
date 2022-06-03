﻿using Luna.Collections;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal interface IRuntimeScope
{
    ValueElement GetConstantValue(string constantName);
    bool IsDeclaredFunction(string functionName);
    string[] GetFunctionArgumentNames(string functionName);
    IRuntimeValue GetFunctionArgumentValue(string argumentName);
    void PushFunctionArguments();
    void PopFunctionArguments();
    void AddFunctionArgument(string argumentName, IRuntimeValue argumentValue);
    IRuntimeValue GetDeclaredFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues);
    bool IsEmbeddedFunction(string functionName);
    IRuntimeValue GetEmbeddedFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues);
    AddLambdaResult AddLambda(LambdaValueElement lambdaElement);
}

internal class AddLambdaResult
{
    public string Name { get; }
    public ReadonlyArray<IRuntimeValue> AlreadyPassedArguments { get; }

    public AddLambdaResult(string name, ReadonlyArray<IRuntimeValue> alreadyPassedArguments)
    {
        Name = name;
        AlreadyPassedArguments = alreadyPassedArguments;
    }
}
