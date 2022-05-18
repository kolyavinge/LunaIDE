using System.Linq;
using Luna.Collections;
using Luna.ProjectModel;

namespace Luna.Runtime;

internal interface IValueElementEvaluator
{
    IRuntimeValue Eval(IRuntimeScope scope, ValueElement element);
}

internal class ValueElementEvaluator : IValueElementEvaluator
{
    public IRuntimeScopesCollection? Scopes { get; set; }

    public IRuntimeValue Eval(IRuntimeScope scope, ValueElement element)
    {
        if (element is BooleanValueElement booleanElement) return new BooleanRuntimeValue(booleanElement.Value);
        if (element is IntegerValueElement integerElement) return new IntegerRuntimeValue(integerElement.Value);
        if (element is FloatValueElement floatElement) return new FloatRuntimeValue(floatElement.Value);
        if (element is StringValueElement stringElement) return new StringRuntimeValue(stringElement.Value);
        if (element is ListValueElement listElement) return new ListRuntimeValue(listElement.Items.Select(item => Eval(scope, item)));
        if (element is NamedConstantValueElement constElement) return Eval(scope, scope.GetConstantValue(constElement.Name));
        if (element is FunctionArgumentValueElement argElement) return scope.GetFunctionArgumentValue(argElement.Name)!;
        if (element is FunctionValueElement funcElement && scope.IsDeclaredFunction(funcElement.Name))
        {
            scope.PushFunctionArguments();
            var funcScope = Scopes!.GetForCodeModel(funcElement.CodeModel);
            var func = new FunctionRuntimeValue(funcElement.Name, funcScope);
            var result = func.GetValue(funcElement.ArgumentValues.Select(arg => Eval(scope, arg)).ToReadonlyArray());
            scope.PopFunctionArguments();
            return result;
        }
        if (element is FunctionValueElement argumentElement) // argument called as function
        {
            var funcScope = Scopes!.GetForCodeModel(argumentElement.CodeModel);
            var func = new FunctionRuntimeValue(argumentElement.Name, funcScope);
            return func.GetValue(argumentElement.ArgumentValues.Select(arg => Eval(scope, arg)).ToReadonlyArray());
        }
        if (element is LambdaValueElement lambdaElement)
        {
            var lambdaScopeName = scope.AddLambda(lambdaElement);
            return new FunctionRuntimeValue(lambdaScopeName, scope);
        }
        throw RuntimeException.CannotConvert(element);
    }
}
