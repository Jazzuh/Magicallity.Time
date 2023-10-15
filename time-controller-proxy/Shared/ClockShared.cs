using System;
using CfxUtils.Logging;
using CitizenFX.Core;
using CitizenFX.Shared.Native;

namespace Magicallity.Time
{
    public partial class Clock : BaseScript
    {
        #region Properties
        #region Day
        /// <summary>
        /// Gets if it is day time
        /// </summary>
        public static bool IsDay => runExportReturnMethod("IsDay", true);

        /// <summary>
        /// Gets if it is night time
        /// </summary>
        public static bool IsNight => runExportReturnMethod("IsNight", false);
        #endregion

        #region Hour
        /// <summary>
        /// Gets or sets the hour that day will start
        /// </summary>
        public static int DayStartHour
        {
            get => runExportReturnMethod("GetDayStartHour", 6);
#if SERVER
            set => runExportMethod("SetDayStartHour", value);
#endif
        }

        /// <summary>
        /// Gets or sets the hour that night will start
        /// </summary>
        public static int NightStartHour
        {
            get => runExportReturnMethod("GetNightStartHour", 21);
#if SERVER
            set => runExportMethod("SetNightStartHour", value);
#endif
        }
        #endregion

        #region Minute
        /// <summary>
        /// Gets the current amount of milliseconds for a minute to pass in the world
        /// </summary>
        public static int MillisecondsPerGameMinute => runExportReturnMethod("GetCurrentMillisecondsPerGameMinute", 2000);

        /// <summary>
        /// Gets or sets the amount of milliseconds it takes for a minute to pass during the day
        /// </summary>
        public static int DayMillisecondsPerGameMinute
        {
            get => runExportReturnMethod("GetDayMillisecondsPerGameMinute", 2000);
#if SERVER
            set => runExportMethod("SetDayMillisecondsPerGameMinute", value);
#endif
        }

        /// <summary>
        /// Gets or sets the amount of milliseconds it takes for a minute to pass at night
        /// </summary>
        public static int NightMillisecondsPerGameMinute
        {
            get => runExportReturnMethod("GetNightMillisecondsPerGameMinute", 2000);
#if SERVER
            set => runExportMethod("SetNightMillisecondsPerGameMinute", value);
#endif
        }
        #endregion

        #region Clock
        /// <summary>
        /// Gets or sets the current date and time
        /// </summary>
        public static DateTime CurrentDate
        {
            get => GetDate();
            set
            {
                SetDate(value.Day, value.Month, value.Year);
                SetTime(value.Hour, value.Minute, value.Second);
            }
        }

        /// <summary>
        /// Gets or sets the current time of day
        /// </summary>
        public static TimeSpan CurrentDayTime
        {
            get => GetTime();
            set => SetTime(value.Hours, value.Minutes, value.Seconds);
        }

        /// <summary>
        /// Gets or sets if the clock is paused
        /// </summary>
        public static bool Paused
        {
            get => runExportReturnMethod("GetClockPaused", false);
            set => SetPaused(value);
        }
        #endregion
        #endregion

        #region Methods
        /// <summary>
        /// Gets the current time of day
        /// </summary>
        protected static partial TimeSpan GetTime();

        /// <summary>
        /// Sets the clock time
        /// </summary>
        /// <param name="hour">The hour to set the clock to</param>
        /// <param name="minute">The minute to set the clock to</param>
        /// <param name="second">The second to set the clock to</param>
        protected static partial void SetTime(int hour, int minute, int second);

        /// <summary>
        /// Gets or sets the current date and time
        /// </summary>
        protected static partial DateTime GetDate();

        /// <summary>
        /// Sets the clock date
        /// </summary>
        /// <param name="day">The day to set the clock to</param>
        /// <param name="month">The month to set the clock to</param>
        /// <param name="year">The second to set the clock to</param>
        protected static partial void SetDate(int day, int month, int year);

        /// <summary>
        /// Sets the clock pause state
        /// </summary>
        /// <param name="state">The state of the clock pause</param>
        protected static partial void SetPaused(bool state);

        private static TReturnType runExportReturnMethod<TReturnType>(string exportName, TReturnType defaultReturnType = default)
        {
            var returnValue = defaultReturnType;

            try
            {
                var exportFunc = Exports.Local["time-controller", exportName];

                returnValue = exportFunc().Result;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"runExportReturnMethod ({exportName.ToYellow()}): An error occurred - {e.StackTrace}");
            }

            return returnValue;
        }

        private static void runExportMethod(string exportName, params object[] args)
        {
            try
            {
                var exportFunc = Exports.Local["time-controller", exportName];

                exportFunc(args);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"runExportMethod ({exportName.ToYellow()}): An error occurred - {e.StackTrace}");
            }
        }
        #endregion
    }
}