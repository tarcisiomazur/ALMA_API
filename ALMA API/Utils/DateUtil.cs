namespace ALMA_API.Utils
{
    public static class DateUtil
    {
        public static int ConvertToTimeStamp(DateTime dateTime)
        {
            var timeStamp = (int)(dateTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return timeStamp;
        }

        public static bool ExpireIn(DateTime dateTime, TimeSpan time)
        {
            return DateTime.Now.AddTicks(time.Ticks) >= dateTime;
        }
    }
}
