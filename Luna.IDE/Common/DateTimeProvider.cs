namespace Luna.IDE.Common;

public interface IDateTimeProvider
{
    DateTime GetNowDateTime();
}

internal class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNowDateTime()
    {
        return DateTime.Now;
    }
}
