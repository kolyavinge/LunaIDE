namespace Luna.IDE.App.Utils;

public class NotInitializedException : Exception
{
    public NotInitializedException(string name)
        : base($"{name} has not been initialized yet.")
    {
    }
}
