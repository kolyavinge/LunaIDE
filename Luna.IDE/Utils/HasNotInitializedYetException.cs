using System;

namespace Luna.IDE.Utils;

public class HasNotInitializedYetException : Exception
{
    public HasNotInitializedYetException(string name)
        : base($"{name} has not initialized yet.")
    {
    }
}
