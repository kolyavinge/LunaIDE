using System.Collections;
using System.Collections.Generic;

namespace Luna.Runtime;

internal class CallStack : IEnumerable<IFunctionRuntimeValue>
{
    private readonly Stack<IFunctionRuntimeValue> _stack = new();

    public int Count => _stack.Count;

    public void Push(IFunctionRuntimeValue function)
    {
        _stack.Push(function);
    }

    public void Pop()
    {
        _stack.Pop();
    }

    public IEnumerator<IFunctionRuntimeValue> GetEnumerator() => _stack.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
