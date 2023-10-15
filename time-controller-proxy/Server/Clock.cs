using System;

namespace Magicallity.Time
{
    public partial class Clock
    {
        #region Methods
        protected static partial TimeSpan GetTime()
        {
            var hour = runExportReturnMethod("GetClockHours", 0);
            var minute = runExportReturnMethod("GetClockMinutes", 0);
            var second = runExportReturnMethod("GetClockSeconds", 0);

            return new TimeSpan(hour, minute, second);
        }

        protected static partial void SetTime(int hour, int minute, int second)
        {
            runExportMethod("SetClockTime", hour, minute, second);
        }

        protected static partial DateTime GetDate()
        {
            var year = runExportReturnMethod("GetClockYear", 2020);
            var month = runExportReturnMethod("GetClockMonth", 1);
            var day = runExportReturnMethod("GetClockDayOfMonth", 1);
            var hour = runExportReturnMethod("GetClockHours", 0);
            var minute = runExportReturnMethod("GetClockMinutes", 0);
            var second = runExportReturnMethod("GetClockSeconds", 0);

            return new DateTime(year, month, day, hour, minute, second);
        }

        protected static partial void SetDate(int day, int month, int year)
        {
            runExportMethod("SetClockDate", day, month, year);
        }

        protected static partial void SetPaused(bool state)
        {
            runExportMethod("SetClockPaused", state);
        }
        #endregion
    }
}