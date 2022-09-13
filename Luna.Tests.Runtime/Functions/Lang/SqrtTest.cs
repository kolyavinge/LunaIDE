using Luna.Functions.Math;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class SqrtTest : BaseFunctionTest<Sqrt>
{
	[Test]
	public void GetValue()
	{
		var integerResult = GetValue<FloatRuntimeValue>(new FloatRuntimeValue(16.0));
		Assert.AreEqual(4.0, integerResult.FloatValue);
	}
}
