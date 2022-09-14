using Luna.Navigation;
using Luna.ProjectModel;
using NUnit.Framework;

namespace Luna.Tests.Navigation;

internal class CodeElementComparerTest
{
	private CodeElementComparer _comparer;

	[SetUp]
	public void Setup()
	{
		_comparer = new CodeElementComparer();
	}

	[Test]
	public void Compare_XNull()
	{
		var result = _comparer.Compare(null, new IntegerValueElement(0));
		Assert.AreEqual(-1, result);
	}

	[Test]
	public void Compare_YNull()
	{
		var result = _comparer.Compare(new IntegerValueElement(0), null);
		Assert.AreEqual(1, result);
	}

	[Test]
	public void Compare_Nulls()
	{
		var result = _comparer.Compare(null, null);
		Assert.AreEqual(0, result);
	}

	[Test]
	public void Compare_ByColumns()
	{
		var result = _comparer.Compare(new IntegerValueElement(0, 0, 1), new IntegerValueElement(0, 0, 2));
		Assert.AreEqual(-1, result);

		result = _comparer.Compare(new IntegerValueElement(0, 0, 2), new IntegerValueElement(0, 0, 1));
		Assert.AreEqual(1, result);

		result = _comparer.Compare(new IntegerValueElement(0, 0, 1), new IntegerValueElement(0, 0, 1));
		Assert.AreEqual(0, result);
	}

	[Test]
	public void Compare_ByLines()
	{
		var result = _comparer.Compare(new IntegerValueElement(0, 0, 1), new IntegerValueElement(0, 10, 2));
		Assert.AreEqual(-1, result);

		result = _comparer.Compare(new IntegerValueElement(0, 10, 2), new IntegerValueElement(0, 0, 1));
		Assert.AreEqual(1, result);

		result = _comparer.Compare(new IntegerValueElement(0, 10, 1), new IntegerValueElement(0, 10, 1));
		Assert.AreEqual(0, result);
	}
}
