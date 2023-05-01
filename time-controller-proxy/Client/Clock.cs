using CitizenFX.Core.Native;
using System;
using CitizenFX.Core;

namespace Magicallity.Time
{
    public partial class Clock
    {
        #region Properties
        /// <summary>
        /// Gets or sets if the clock is being manually controlled from the client side
        /// </summary>
        public static bool ManuallyControl
        {
            get => runExportReturnMethod(() => _timeExport.GetManualTimeControl(), false);
            set => runExportMethod(() => _timeExport.SetManualTimeControl(value));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Syncs the clock with the global state
        /// </summary>
        public static void Sync()
        {
            runExportMethod(() => _timeExport.Sync());
        }

        protected static partial TimeSpan GetTime()
        {
            var retval = World.CurrentDayTime;

            if (!ManuallyControl)
            {
                var hour = runExportReturnMethod(() => _timeExport.GetClockHours(), 0);
                var minute = runExportReturnMethod(() => _timeExport.GetClockMinutes(), 0);
                var second = runExportReturnMethod(() => _timeExport.GetClockSeconds(), 0);

                retval = new TimeSpan(hour, minute, second);
            }

            return retval;
        }

        protected static partial void SetTime(int hour, int minute, int second)
        {
            if (!ManuallyControl)
            {
                return;
            }

            API.NetworkOverrideClockTime(hour, minute, second);
        }

        protected static partial DateTime GetDate()
        {
            var retval = World.CurrentDate;

            if (!ManuallyControl)
            {
                var year = runExportReturnMethod(() => _timeExport.GetClockYear(), 2020);
                var month = runExportReturnMethod(() => _timeExport.GetClockMonth(), 1);
                var day = runExportReturnMethod(() => _timeExport.GetClockDayOfMonth(), 1);
                var hour = runExportReturnMethod(() => _timeExport.GetClockHours(), 0);
                var minute = runExportReturnMethod(() => _timeExport.GetClockMinutes(), 0);
                var second = runExportReturnMethod(() => _timeExport.GetClockSeconds(), 0);

                retval = new DateTime(year, month, day, hour, minute, second);
            }

            return retval;
        }

        protected static partial void SetDate(int day, int month, int year)
        {
            if (!ManuallyControl)
            {
                return;
            }

            API.SetClockDate(day, month, year);
        }

        protected static partial void SetPaused(bool state)
        {
            ManuallyControl = state;
        }
        #endregion

    }
}