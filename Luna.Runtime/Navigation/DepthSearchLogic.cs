using System.Collections.Generic;
using System.Linq;
using Luna.CodeElements;

namespace Luna.Navigation;

internal class DepthSearchLogic
{
    private readonly Stack<CodeElement> _chain;
    private CodeElement? _result;
    private Predicate<CodeElement> _criteria;

    public DepthSearchLogic()
    {
        _chain = new();
        _criteria = _ => false;
    }

    public SearchResult? Seach(CodeElement root, Predicate<CodeElement> criteria)
    {
        _criteria = criteria;
        _chain.Clear();
        if (SeachRec(root))
        {
            return new(_result!, _chain);
        }
        else
        {
            return null;
        }
    }

    private bool SeachRec(CodeElement parent)
    {
        if (_criteria(parent))
        {
            _result = parent;
            return true;
        }

        if (parent is ConstantDeclaration constantDeclaration)
        {
            if (SeachRec(constantDeclaration.Value)) { _chain.Push(parent); return true; }
        }
        else if (parent is FunctionDeclaration functionDeclaration)
        {
            foreach (var argument in functionDeclaration.Arguments)
            {
                if (SeachRec(argument)) { _chain.Push(parent); return true; }
            }
            foreach (var bodyItem in functionDeclaration.Body)
            {
                if (SeachRec(bodyItem)) { _chain.Push(parent); return true; }
            }
        }
        else if (parent is ListValueElement listValueElement)
        {
            foreach (var item in listValueElement.Items)
            {
                if (SeachRec(item)) { _chain.Push(parent); return true; }
            }
        }
        else if (parent is LambdaValueElement lambdaValueElement)
        {
            foreach (var argument in lambdaValueElement.Arguments)
            {
                if (SeachRec(argument)) { _chain.Push(parent); return true; }
            }
            foreach (var bodyItem in lambdaValueElement.Body)
            {
                if (SeachRec(bodyItem)) { _chain.Push(parent); return true; }
            }
        }
        else if (parent is FunctionValueElement functionValueElement)
        {
            foreach (var argumentValue in functionValueElement.ArgumentValues)
            {
                if (SeachRec(argumentValue)) { _chain.Push(parent); return true; }
            }
        }

        return false;
    }

    public class SearchResult
    {
        public readonly CodeElement CodeElement;
        public readonly CodeElement[] Chain;

        public SearchResult(CodeElement codeElement, IEnumerable<CodeElement> chain)
        {
            CodeElement = codeElement;
            Chain = chain.ToArray();
        }
    }
}
