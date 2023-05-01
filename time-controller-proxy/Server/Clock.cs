using System;

namespace Magicallity.Time
{
    public partial class Clock
    {
        #region Methods
        protected static partial TimeSpan GetTime()
        {
            var hour = runExportReturnMethod(() => _timeExport.GetClockHours(), 0);
            var minute = runExportReturnMethod(() => _timeExport.GetClockMinutes(), 0);
            var second = runExportReturnMethod(() => _timeExport.GetClockSeconds(), 0);

            return new TimeSpan(hour, minute, second);
        }

        protected static partial void SetTime(int hour, int minute, int second)
        {
            runExportMethod(() => _timeExport.SetClockTime(hour, minute, second));
        }

        protected static partial DateTime GetDate()
        {
            var year = runExportReturnMethod(() => _timeExport.GetClockYear(), 2020);
            var month = runExportReturnMethod(() => _timeExport.GetClockMonth(), 1);
            var day = runExportReturnMethod(() => _timeExport.GetClockDayOfMonth(), 1);
            var hour = runExportReturnMethod(() => _timeExport.GetClockHours(), 0);
            var minute = runExportReturnMethod(() => _timeExport.GetClockMinutes(), 0);
            var second = runExportReturnMethod(() => _timeExport.GetClockSeconds(), 0);

            return new DateTime(year, month, day, hour, minute, second);
        }

        protected static partial void SetDate(int day, int month, int year)
        {
            runExportMethod(() => _timeExport.SetClockDate(day, month, year));
        }

        protected static partial void SetPaused(bool state)
        {
            runExportMethod(() => _timeExport.SetClockPaused(state));
        }
        #endregion
    }
}