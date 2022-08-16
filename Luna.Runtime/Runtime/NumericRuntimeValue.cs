namespace Luna.Runtime;

internal abstract class NumericRuntimeValue : RuntimeValue
{
    public long IntegerValue { get; }

    public double FloatValue { get; }

    protected NumericRuntimeValue(long value)
    {
        IntegerValue = value;
        FloatValue = value;
    }

    protected NumericRuntimeValue(double value)
    {
        IntegerValue = (int)value;
        FloatValue = value;
    }

    public override bool Equals(object? obj)
    {
        return obj is NumericRuntimeValue value &&
               Math.Abs(FloatValue - value.FloatValue) < 0.0001;
    }

    public override int GetHashCode()
    {
        return FloatValue.GetHashCode();
    }
}
