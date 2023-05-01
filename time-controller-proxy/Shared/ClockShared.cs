using System;
using CfxUtils.Shared.Logging;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace Magicallity.Time
{
    public partial class Clock : BaseScript
    {
        #region Properties
        #region Day
        /// <summary>
        /// Gets if it's currently day
        /// </summary>
        public static bool IsDay => runExportReturnMethod(() => _timeExport.IsDay(), true);

        /// <summary>
        /// Gets if it's currently night
        /// </summary>
        public static bool IsNight => runExportReturnMethod(() => _timeExport.IsNight(), false);
        #endregion

        #region Hour
        /// <summary>
        /// Gets or sets the hour that day starts
        /// </summary>
        public static int DayStartHour
        {
            get => runExportReturnMethod(() => _timeExport.GetDayStartHour(), 6);
#if SERVER
            set => runExportMethod(() => _timeExport.SetDayStartHour(value));
#endif
        }

        /// <summary>
        /// Gets or sets the hour that night will start
        /// </summary>
        public static int NightStartHour
        {
            get => runExportReturnMethod(() => _timeExport.GetNightStartHour(), 21);
#if SERVER
            set => runExportMethod(() => _timeExport.SetNightStartHour(value));
#endif
        }
        #endregion

        #region Minute
        /// <summary>
        /// Gets the current amount of milliseconds for a minute to pass in the world
        /// </summary>
        public static int MillisecondsPerGameMinute => runExportReturnMethod(() => _timeExport.GetCurrentMillisecondsPerGameMinute(), 2000);

        /// <summary>
        /// Gets or sets the amount of milliseconds it takes for a minute to pass during the day
        /// </summary>
        public static int DayMillisecondsPerGameMinute
        {
            get => runExportReturnMethod(() => _timeExport.GetDayMillisecondsPerGameMinute(), 2000);
#if SERVER
            set => runExportMethod(() => _timeExport.SetDayMillisecondsPerGameMinute(value));
#endif
        }

        /// <summary>
        /// Gets or sets the amount of milliseconds it takes for a minute to pass at night
        /// </summary>
        public static int NightMillisecondsPerGameMinute
        {
            get => runExportReturnMethod(() => _timeExport.GetNightMillisecondsPerGameMinute(), 2000);
#if SERVER
            set => runExportMethod(() => _timeExport.SetNightMillisecondsPerGameMinute(value));
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
            get => runExportReturnMethod(() => _timeExport.GetClockPaused(), false);
            set => SetPaused(value);
        }
        #endregion
        #endregion

        #region Fields
        private static dynamic _timeExport;
        private static bool _timeControllerActive;
        #endregion

        #region Constructor
        public Clock()
        {
            _timeExport = Exports["time-controller"];
            _timeControllerActive = API.GetResourceState("time-controller") is "started" or "starting";
        }
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

        private static TReturnType runExportReturnMethod<TReturnType>(Func<TReturnType> exportFunc, TReturnType defaultReturnType = default)
        {
            var returnValue = defaultReturnType;

            if (_timeControllerActive)
            {
                try
                {
                    returnValue = exportFunc();
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
            else
            {
                Log.Warning($"Resource {LogColors.Yellow}time-controller{LogColors.Reset} is detected as not running, not running export");
            }

            return returnValue;
        }

        private static void runExportMethod(Action exportFunc)
        {
            if (!_timeControllerActive)
            {
                Log.Warning($"Resource {LogColors.Yellow}time-controller{LogColors.Reset} is detected as not running, not running export");
                return;
            }

            try
            {
                exportFunc.DynamicInvoke();
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
        #endregion

        #region Events
        [EventHandler("onResourceStart")]
        private void OnResourceStart(string resourceName)
        {
            if (resourceName == "time-controller")
            {
                _timeControllerActive = true;
            }
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (resourceName == "time-controller")
            {
                _timeControllerActive = false;
            }
        }
        #endregion
    }
}