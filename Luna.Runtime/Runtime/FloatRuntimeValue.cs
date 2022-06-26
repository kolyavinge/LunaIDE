using System.Globalization;

namespace Luna.Runtime;

internal class FloatRuntimeValue : NumericRuntimeValue
{
    public FloatRuntimeValue(double value) : base(value)
    {
    }

    public override string ToString()
    {
        return FloatValue.ToString(new NumberFormatInfo { NumberDecimalSeparator = "." });
    }
}
