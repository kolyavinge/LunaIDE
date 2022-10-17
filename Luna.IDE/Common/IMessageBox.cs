namespace Luna.IDE.Common;

public enum MessageBoxButtons
{
    YesNo
}

public enum MessageBoxResult
{
    Yes, No
}

public interface IMessageBox
{
    MessageBoxResult? Show(string title, string message, MessageBoxButtons buttons);
}
