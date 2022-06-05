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
        if (element is FunctionValueElement funcElement && scope.IsDeclaredOrEmbeddedFunction(funcElement.Name))
        {
            var arguments = funcElement.ArgumentValues.Select(arg => Eval(scope, arg)).ToReadonlyArray();
            var funcScope = Scopes!.GetForCodeModel(funcElement.CodeModel);
            return new FunctionRuntimeValue(funcElement.Name, funcScope) { AlreadyPassedArguments = arguments };
        }
        if (element is FunctionValueElement argFuncElement)
        {
            var value = scope.GetFunctionArgumentValue(argFuncElement.Name);
            if (value is FunctionRuntimeValue funcArgValue)
            {
                var arguments = argFuncElement.ArgumentValues.Select(arg => Eval(scope, arg)).ToReadonlyArray();
                return funcArgValue.GetValue(arguments);
            }
            else throw RuntimeException.IsNotFunction(argFuncElement.Name);
        }
        if (element is LambdaValueElement lambdaElement)
        {
            var result = scope.AddLambda(lambdaElement);
            return new FunctionRuntimeValue(result.Name, scope) { AlreadyPassedArguments = result.AlreadyPassedArguments };
        }
        throw RuntimeException.CannotConvert(element);
    }
}
