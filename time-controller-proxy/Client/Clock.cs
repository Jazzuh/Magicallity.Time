using System;
using CitizenFX.FiveM;
using CitizenFX.FiveM.Native;

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
            get => runExportReturnMethod("GetManualTimeControl", false);
            set => runExportMethod("SetManualTimeControl", value);
        }
        #endregion

        #region Methods
        /// <summary>
        /// Syncs the clock with the global state
        /// </summary>
        public static void Sync()
        {
            runExportMethod("Sync");
        }

        protected static partial TimeSpan GetTime()
        {
            var retval = World.CurrentDayTime;

            if (!ManuallyControl)
            {
                var hour = runExportReturnMethod("GetClockHours", 0);
                var minute = runExportReturnMethod("GetClockMinutes", 0);
                var second = runExportReturnMethod("GetClockSeconds", 0);

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

            Natives.NetworkOverrideClockTime(hour, minute, second);
        }

        protected static partial DateTime GetDate()
        {
            var retval = World.CurrentDate;

            if (!ManuallyControl)
            {
                var year = runExportReturnMethod("GetClockYear", 2020);
                var month = runExportReturnMethod("GetClockMonth", 1);
                var day = runExportReturnMethod("GetClockDayOfMonth", 1);
                var hour = runExportReturnMethod("GetClockHours", 0);
                var minute = runExportReturnMethod("GetClockMinutes", 0);
                var second = runExportReturnMethod("GetClockSeconds", 0);

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

            Natives.SetClockDate(day, month, year);
        }

        protected static partial void SetPaused(bool state)
        {
            ManuallyControl = state;
        }
        #endregion

    }
}