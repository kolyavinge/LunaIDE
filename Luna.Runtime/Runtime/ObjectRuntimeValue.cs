namespace Luna.Runtime;

internal class ObjectRuntimeValue : RuntimeValue
{
    private readonly object _innerObject;

    public ObjectRuntimeValue(object innerObject)
    {
        _innerObject = innerObject;
    }

    public TInnerObject Get<TInnerObject>()
    {
        return (TInnerObject)_innerObject;
    }
}
