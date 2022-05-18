using Luna.Collections;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal interface IRuntimeScope
{
    ValueElement GetConstantValue(string constantName);
    bool ArgumentCalledAsFunction(string argumentName);
    bool IsDeclaredFunction(string functionName);
    string[] GetFunctionArgumentNames(string functionName);
    IRuntimeValue GetFunctionArgumentValue(string argumentName);
    void PushFunctionArguments();
    void PopFunctionArguments();
    void AddFunctionArgument(string argumentName, IRuntimeValue argumentValue);
    void RemoveFunctionArgument(string argumentName);
    IRuntimeValue GetDeclaredFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues);
    bool IsEmbeddedFunction(string functionName);
    IRuntimeValue GetEmbeddedFunctionValue(string functionName, ReadonlyArray<IRuntimeValue> argumentValues);
    string AddLambda(LambdaValueElement lambdaElement);
}
