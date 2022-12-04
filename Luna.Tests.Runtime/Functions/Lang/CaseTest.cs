using Luna.Functions.Lang;
using Luna.Runtime;
using Luna.Tests.Tools;
using NUnit.Framework;

namespace Luna.Tests.Functions.Lang;

internal class CaseTest : BaseFunctionTest<Case>
{
    [SetUp]
    public void Setup()
    {
        Init();
    }

    [Test]
    public void GetValue()
    {
        var result = GetValue<IntegerRuntimeValue>(
            new IntegerRuntimeValue(2),
            new ListRuntimeValue(new IRuntimeValue[]
            {
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(10) }),
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new IntegerRuntimeValue(20) }),
                new IntegerRuntimeValue(30)
            }));

        Assert.That(result.IntegerValue, Is.EqualTo(20));
    }

    [Test]
    public void GetValue_DefaultCase()
    {
        var result = GetValue<IntegerRuntimeValue>(
            new IntegerRuntimeValue(777),
            new ListRuntimeValue(new IRuntimeValue[]
            {
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(10) }),
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new IntegerRuntimeValue(20) }),
                new IntegerRuntimeValue(30)
            }));

        Assert.That(result.IntegerValue, Is.EqualTo(30));
    }

    [Test]
    public void GetValue_OnlyDefaultCase()
    {
        var result = GetValue<IntegerRuntimeValue>(
            new IntegerRuntimeValue(777),
            new ListRuntimeValue(new IRuntimeValue[]
            {
                new IntegerRuntimeValue(30)
            }));

        Assert.That(result.IntegerValue, Is.EqualTo(30));
    }

    [Test]
    public void GetValue_WrongValueType_DefaultCase()
    {
        var result = GetValue<IntegerRuntimeValue>(
            new StringRuntimeValue("1"),
            new ListRuntimeValue(new IRuntimeValue[]
            {
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(1), new IntegerRuntimeValue(10) }),
                new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new IntegerRuntimeValue(20) }),
                new IntegerRuntimeValue(30)
            }));

        Assert.That(result.IntegerValue, Is.EqualTo(30));
    }

    [Test]
    public void IncorrectItem()
    {
        try
        {
            GetValue<VoidRuntimeValue>(
                new IntegerRuntimeValue(1),
                new ListRuntimeValue(new IRuntimeValue[]
                {
                    new IntegerRuntimeValue(10),
                    new ListRuntimeValue(new IRuntimeValue[] { new IntegerRuntimeValue(2), new IntegerRuntimeValue(20) }),
                    new IntegerRuntimeValue(30)
                }));

            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("Items in cases must be a list with two items.")));
        }
    }

    [Test]
    public void EmptyCases_Error()
    {
        try
        {
            GetValue<VoidRuntimeValue>(
                new IntegerRuntimeValue(1),
                new ListRuntimeValue(new IRuntimeValue[0]));

            Assert.Fail();
        }
        catch (RuntimeException rte)
        {
            Assert.That(rte, Is.EqualTo(new RuntimeException("Cases cannot be empty.")));
        }
    }
}
