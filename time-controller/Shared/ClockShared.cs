using System;
using CitizenFX.Core;

namespace Magicallity.Time.Shared
{
    public class Clock : BaseScript
    {
        #region Year
        [Export("GetClockYear")]
        public static int GetClockYear()
        {
            return getTime().Year;
        }
        #endregion

        #region Month
        [Export("GetClockMonth")]
        public static int GetClockMonth()
        {
            return getTime().Month;
        }
        #endregion

        #region Day
        /// <summary>
        /// Gets if it is day
        /// </summary>
        /// <returns>If it is day</returns>
        [Export("IsDay")]
        public static bool IsDay()
        {
            var currentHour = GetHours();

            return currentHour >= GetDayStartHour() && currentHour < GetNightStartHour();
        }

        /// <summary>
        /// Gets if it is currently night
        /// </summary>
        /// <returns>If it is night</returns>
        [Export("IsNight")]
        public static bool IsNight()
        {
            return !IsDay();
        }

        /// <summary>
        /// Gets the current day
        /// </summary>
        /// <returns>The current day</returns>
        [Export("GetClockDayOfMonth")]
        public static int GetDayOfMonth()
        {
            return getTime().Day;
        }

        [Export("GetClockDayOfWeek")]
        public static int GetDayOfWeek()
        {
            return (int)getTime().DayOfWeek;
        }
        #endregion

        #region Hour
        /// <summary>
        /// Gets the current hour
        /// </summary>
        /// <returns>The current hour. 0-23</returns>
        [Export("GetClockHours")]
        public static int GetHours()
        {
            return getTime().Hour;
        }

        /// <summary>
        /// Gets the current minute
        /// </summary>
        /// <returns>The current minute. 0-59</returns>
        [Export("GetDayStartHour")]
        public static int GetDayStartHour()
        {
            return StateBag.Global["clock:day:start"] ?? 6;
        }

        /// <summary>
        /// Gets the hour which indicates the start of the night
        /// </summary>
        /// <returns>The hour which marks the start of the night</returns>
        [Export("GetNightStartHour")]
        public static int GetNightStartHour()
        {
            return StateBag.Global["clock:night:start"] ?? 21;
        }
        #endregion

        #region Minute
        /// <summary>
        /// Gets the current minute
        /// </summary>
        /// <returns>The current minute</returns>
        [Export("GetClockMinutes")]
        public static int GetMinutes()
        {
            return getTime().Minute;
        }

        /// <summary>
        /// Gets the amount of milliseconds that a game minute will take during the day
        /// </summary>
        /// <returns>The amount of milliseconds a game minute during the day will take</returns>
        [Export("GetDayMillisecondsPerGameMinute")]
        public static int GetDayMillisecondsPerGameMinute()
        {
            return StateBag.Global["clock:day:msPerMinute"] ?? 2000;
        }

        /// <summary>
        /// Gets the amount of milliseconds that a game minute will take during the night
        /// </summary>
        /// <returns>The amount of milliseconds a game minute during the night will take</returns>
        [Export("GetNightMillisecondsPerGameMinute")]
        public static int GetNightMillisecondsPerGameMinute()
        {
            return StateBag.Global["clock:night:msPerMinute"] ?? 2000;
        }

        /// <summary>
        /// Gets the current amount of milliseconds that a game minute will take
        /// </summary>
        /// <returns>The amount of milliseconds that a game minute will take</returns>
        [Export("GetCurrentMillisecondsPerGameMinute")]
        public static int GetCurrentMillisecondsPerGameMinute()
        {
            return IsDay() ? GetDayMillisecondsPerGameMinute() : GetNightMillisecondsPerGameMinute();
        }
        #endregion

        #region Second
        /// <summary>
        /// Gets the current second
        /// </summary>
        /// <returns>The current second</returns>
        [Export("GetClockSeconds")]
        public static int GetSeconds()
        {
            return getTime().Second;
        }
        #endregion

        #region Clock Freezing
        /// <summary>
        /// Gets if the clock is currently paused
        /// </summary>
        /// <returns>If the clock is currently paused</returns>
        [Export("GetClockPaused")]
        public static bool GetPaused()
        {
            return StateBag.Global["clock:paused"] ?? false;
        }
        #endregion

        #region Time Getter
        private static DateTime getTime()
        {
            var timeTickString = StateBag.Global["clock:ticks"] ?? "0";
            var timeTick = long.Parse(timeTickString);

            return new DateTime(timeTick);
        }
        #endregion
    }
}
