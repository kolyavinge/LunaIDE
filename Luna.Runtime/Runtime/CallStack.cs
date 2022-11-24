using System.Collections;
using System.Collections.Generic;

namespace Luna.Runtime;

internal class CallStack : IEnumerable<IFunctionRuntimeValue>
{
    public const int MaxDepth = 1000;

    private readonly Stack<IFunctionRuntimeValue> _stack = new();

    public int Count => _stack.Count;

    public void Push(IFunctionRuntimeValue function)
    {
        if (Count == MaxDepth) throw RuntimeException.Stackoverflow();
        _stack.Push(function);
    }

    public void Pop()
    {
        _stack.Pop();
    }

    public IEnumerator<IFunctionRuntimeValue> GetEnumerator() => _stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
