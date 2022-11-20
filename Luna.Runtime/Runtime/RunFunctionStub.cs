namespace Luna.Runtime;

internal class RunFunctionStub : IFunctionRuntimeValue
{
    public string Name => "run";

    public override string ToString() => Name;
}
