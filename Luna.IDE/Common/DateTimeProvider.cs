namespace Luna.IDE.Common;

public interface IDateTimeProvider
{
    DateTime GetNowDateTime();
}

public class DateTimeProvider : IDateTimeProvider
{
    public DateTime GetNowDateTime()
    {
        return DateTime.Now;
    }
}
