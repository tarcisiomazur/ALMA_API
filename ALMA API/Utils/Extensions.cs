namespace ALMA_API.Utils;

public static class Extensions
{
    public static bool IsNull(this DateTime? datetime)
    {
        return datetime is null;
    }
}