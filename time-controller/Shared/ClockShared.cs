using System;
using CitizenFX.Core;

namespace Magicallity.Time.Shared
{
    public class Clock : BaseScript
    {
        #region Fields
        private static Clock _instance;
        #endregion

        #region Constructor
        public Clock()
        {
            _instance = this;

            Exports.Add("GetClockYear", new Func<int>(GetClockYear));

            Exports.Add("GetClockMonth", new Func<int>(GetClockMonth));

            Exports.Add("IsDay", new Func<bool>(IsDay));
            Exports.Add("IsNight", new Func<bool>(IsNight));
            Exports.Add("GetClockDayOfMonth", new Func<int>(GetDayOfMonth));
            Exports.Add("GetClockDayOfWeek", new Func<int>(GetDayOfWeek));

            Exports.Add("GetClockHours", new Func<int>(GetHours));
            Exports.Add("GetDayStartHour", new Func<int>(GetDayStartHour));    
            Exports.Add("GetNightStartHour", new Func<int>(GetNightStartHour));  
            
            Exports.Add("GetClockMinutes", new Func<int>(GetMinutes));
            Exports.Add("GetDayMillisecondsPerGameMinute", new Func<int>(GetDayMillisecondsPerGameMinute));    
            Exports.Add("GetNightMillisecondsPerGameMinute", new Func<int>(GetNightMillisecondsPerGameMinute));    
            Exports.Add("GetCurrentMillisecondsPerGameMinute", new Func<int>(GetCurrentMillisecondsPerGameMinute));   

            Exports.Add("GetClockSeconds", new Func<int>(GetSeconds));
            
            Exports.Add("GetClockPaused", new Func<bool>(GetPaused));    
        }
        #endregion

        #region Year
        public static int GetClockYear()
        {
            return getTime().Year;
        }
        #endregion

        #region Month
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
        public static bool IsDay()
        {
            var currentHour = GetHours();

            return currentHour >= GetDayStartHour() && currentHour < GetNightStartHour();
        }

        /// <summary>
        /// Gets if it is currently night
        /// </summary>
        /// <returns>If it is night</returns>
        public static bool IsNight()
        {
            return !IsDay();
        }

        /// <summary>
        /// Gets the current day
        /// </summary>
        /// <returns>The current day</returns>
        public static int GetDayOfMonth()
        {
            return getTime().Day;
        }

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
        public static int GetHours()
        {
            return getTime().Hour;
        }

        /// <summary>
        /// Gets the current minute
        /// </summary>
        /// <returns>The current minute. 0-59</returns>
        public static int GetDayStartHour()
        {
            return _instance.GlobalState["clock:day:start"] ?? 6;
        }
        
        /// <summary>
        /// Gets the hour which indicates the start of the night
        /// </summary>
        /// <returns>The hour which marks the start of the night</returns>
        public static int GetNightStartHour()
        {
            return _instance.GlobalState["clock:night:start"] ?? 21;
        }
        #endregion
        
        #region Minute
        /// <summary>
        /// Gets the current minute
        /// </summary>
        /// <returns>The current minute</returns>
        public static int GetMinutes()
        {
            return getTime().Minute;
        }

        /// <summary>
        /// Gets the amount of milliseconds that a game minute will take during the day
        /// </summary>
        /// <returns>The amount of milliseconds a game minute during the day will take</returns>
        public static int GetDayMillisecondsPerGameMinute()
        {
            return _instance.GlobalState["clock:day:msPerMinute"] ?? 2000;
        }

        /// <summary>
        /// Gets the amount of milliseconds that a game minute will take during the night
        /// </summary>
        /// <returns>The amount of milliseconds a game minute during the night will take</returns>
        public static int GetNightMillisecondsPerGameMinute()
        {
            return _instance.GlobalState["clock:night:msPerMinute"] ?? 2000;
        }

        /// <summary>
        /// Gets the current amount of milliseconds that a game minute will take
        /// </summary>
        /// <returns>The amount of milliseconds that a game minute will take</returns>
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
        public static bool GetPaused()
        {
            return _instance.GlobalState["clock:paused"] ?? false;
        }
        #endregion

        #region Time Getter
        private static DateTime getTime()
        {
            var timeTickString = _instance.GlobalState["clock:ticks"] ?? "0";
            var timeTick = long.Parse(timeTickString);

            return new DateTime(timeTick);
        }
        #endregion
    }
}
