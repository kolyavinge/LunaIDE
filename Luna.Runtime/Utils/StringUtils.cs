namespace Luna.Utils;

static class StringUtils
{
    public static bool StringEquals(string? x, char[]? y, int yArrayLength)
    {
        if (x == null || y == null) return false;
        var length = Math.Min(x.Length, y.Length);
        if (length != yArrayLength) return false;
        for (int i = 0; i < length; i++)
        {
            if (x[i] != y[i]) return false;
        }

        return true;
    }
}
