using System;
using System.Linq;
using System.Threading.Tasks;
using CitizenFX.Core;
using CfxUtils.Shared.Convar;
using CfxUtils.Shared.Logging;
using Magicallity.Time.Shared;
using static CitizenFX.Core.Native.API;

namespace Magicallity.Time.Server
{
    public class TimeController : BaseScript
    {
        #region Fields
        private Convar<bool> _isClockPausedConvar = new Convar<bool>("clock_paused", false);
        private Convar<int> _dayStartHourConvar = new Convar<int>("clock_day_start_hour", 6);
        private Convar<int> _nightHourStartConvar = new Convar<int>("clock_night_start_hour", 21);
        private Convar<int> _msPerMinuteDuringDayConvar = new Convar<int>("clock_day_milliseconds_per_minute", 2000);
        private Convar<int> _msPerMinuteDuringNightConvar = new Convar<int>("clock_night_milliseconds_per_minute", 2000);

        private DateTime _currentTime;

        private int _currentYear;
        private int _currentMonth;
        private int _currentDay;
        private int _currentHour;
        #endregion

        #region Constructor
        public TimeController()
        {
            GlobalState["clock:day:msPerMinute"] = _msPerMinuteDuringDayConvar.Value;
            GlobalState["clock:day:start"] = _dayStartHourConvar.Value;

            GlobalState["clock:night:msPerMinute"] = _msPerMinuteDuringNightConvar.Value;
            GlobalState["clock:night:start"] = _nightHourStartConvar.Value;

            GlobalState["clock:paused"] = _isClockPausedConvar.Value;

            var timeTickString = GetResourceKvpString("clock:ticks") ?? "0";
            var timeTick = long.Parse(timeTickString);

            _currentTime = timeTick == 0 ? DateTime.Now : new DateTime(timeTick);

            Sync();

            if (!Clock.GetPaused())
            {
                Tick += UpdateTimeTick;
            }

            Exports.Add("SetDayStartHour", new Action<int>(SetDayStartHour));
            Exports.Add("SetNightStartHour", new Action<int>(SetNightStartHour));

            Exports.Add("SetDayMillisecondsPerGameMinute", new Action<int>(SetDayMillisecondsPerGameMinute));
            Exports.Add("SetNightMillisecondsPerGameMinute", new Action<int>(SetNightMillisecondsPerGameMinute));

            Exports.Add("SetClockTime", new Action<int, int, int>(SetClockTime));
            Exports.Add("SetClockDate", new Action<int, int, int>(SetClockDate));

            Exports.Add("PauseClock", new Action<bool>(PauseClock));
        }
        #endregion

        #region Sync and Save
        /// <summary>
        /// Syncs the clock state to all clients
        /// </summary>
        public void Sync()
        {
            Log.Debug($"Syncing clock state to all clients");

            GlobalState["clock:ticks"] = _currentTime.Ticks.ToString();

            _currentYear = _currentTime.Year;
            _currentMonth = _currentTime.Month;
            _currentDay = _currentTime.Day;
            _currentHour = _currentTime.Hour;
        }

        /// <summary>
        /// Saves the clock state to the database
        /// </summary>
        public void Save()
        {
            Log.Debug($"Saving clock state as {LogColors.Cyan}{_currentTime:G}{LogColors.Reset}");

            SetResourceKvp("clock:ticks", _currentTime.Ticks.ToString());
        }

        /// <summary>
        /// Syncs and saves the clock state
        /// </summary>
        public void SaveAndSync()
        {
            Sync();
            Save();
        }
        #endregion

        #region Hour
        /// <summary>
        /// Gets the hour which indicates the start of the day
        /// </summary>
        /// <param name="newHour">The hour which marks the start of the day</param>
        public void SetDayStartHour(int newHour)
        {
            newHour = MathUtil.Clamp(newHour, 0, 23);

            GlobalState["clock:day:start"] = newHour;
            _dayStartHourConvar.Value = newHour;
        }

        /// <summary>
        /// Gets the hour which indicates the start of the night
        /// </summary>
        /// <param name="newHour">The hour which marks the start of the night</param>
        public void SetNightStartHour(int newHour)
        {
            newHour = MathUtil.Clamp(newHour, 0, 23);

            GlobalState["clock:night:start"] = newHour;
            _nightHourStartConvar.Value = newHour;
        }
        #endregion

        #region Minute
        /// <summary>
        /// Sets the amount of milliseconds that a game minute will take during the day
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds a game minute during the day will take</param>
        public void SetDayMillisecondsPerGameMinute(int milliseconds)
        {
            GlobalState["clock:day:msPerMinute"] = milliseconds;
            _msPerMinuteDuringDayConvar.Value = milliseconds;
        }

        /// <summary>
        /// Sets the amount of milliseconds that a game minute will take during the night
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds a game minute during the day will take</param>
        public void SetNightMillisecondsPerGameMinute(int milliseconds)
        {
            GlobalState["clock:night:msPerMinute"] = milliseconds;
            _msPerMinuteDuringNightConvar.Value = milliseconds;
        }
        #endregion

        #region Clock
        /// <summary>
        /// Sets the clock time
        /// </summary>
        /// <param name="hour">The hour to set the clock to</param>
        /// <param name="minute">The minute to set the clock to</param>
        /// <param name="second">The second to set the clock to</param>
        public void SetClockTime(int hour, int minute, int second)
        {
            _currentTime = new DateTime(_currentTime.Year, _currentTime.Month, _currentTime.Day, hour, minute, second);

            SaveAndSync();
        }

        /// <summary>
        /// Sets the clock date
        /// </summary>
        /// <param name="day">The day to set the clock to</param>
        /// <param name="month">The month to set the clock to</param>
        /// <param name="year">The year to set the clock to</param>
        public void SetClockDate(int day, int month, int year)
        {
            _currentTime = new DateTime(year, month, day, _currentTime.Hour, _currentTime.Minute, _currentTime.Second);

            SaveAndSync();
        }

        /// <summary>
        /// Sets the clock pause state
        /// </summary>
        /// <param name="state">The state of the clock pause</param>
        public void PauseClock(bool state)
        {
            GlobalState["clock:paused"] = state;

            _isClockPausedConvar.Value = state;

            Tick -= UpdateTimeTick;

            if (!state)
            {
                Tick += UpdateTimeTick;
            }
        }
        #endregion

        #region Commands, Events and Ticks
        [Command("time", Restricted = true)]
        private void OnTimeCommand(string[] args) => DoEditCommand(args);

        [Command("clock", Restricted = true)]
        private void OnClockCommand(string[] args) => DoEditCommand(args);

        // TODO use [Export] once mono_v2 has been implemented
        private void DoEditCommand(string[] args)
        {
            var subCommand = args.GetArgAs(0, "").ToLower();

            if (subCommand is "set")
            {
                SetClockTime(args.GetArgAs(1, 0), args.GetArgAs(2, 0), args.GetArgAs(3, 0));

                Log.Information($"Set clock to: {LogColors.Yellow}{Clock.GetHours():D2}:{Clock.GetMinutes():D2}{LogColors.Reset}");
            }
            else if (subCommand is "get")
            {
                Log.Information($"Current clock is: {LogColors.Yellow}{Clock.GetHours():D2}:{Clock.GetMinutes():D2}{LogColors.Reset}");
            }
            else if (subCommand is "pause" or "freeze")
            {
                var pauseArg = args.GetArgAs(1, "");

                switch (pauseArg)
                {
                    case "true":
                        PauseClock(true);
                        break;
                    case "false":
                        PauseClock(false);
                        break;
                    default:
                        PauseClock(!Clock.GetPaused());
                        break;
                }

                Log.Information($"Clock has been {LogColors.Yellow}{(Clock.GetPaused() ? "paused" : "unpaused")}{LogColors.Reset}");
            }
            else if (subCommand is "sync")
            {
                Sync();
            }
            else if (subCommand is "save")
            {
                Save();
            }
            else if (subCommand is "day" or "night")
            {
                var subCommandCapitalized = char.ToUpper(subCommand.First()) + subCommand.Substring(1);
                var setType = args.GetArgAs(1, "");

                switch (setType)
                {
                    case "start":
                        var startAction = args.GetArgAs(2, "get");
                        var startHour = subCommand == "day" ? Clock.GetDayStartHour() : Clock.GetNightStartHour();

                        switch (startAction)
                        {
                            case "get":
                                Log.Information($"{subCommandCapitalized} currently starts at {LogColors.Yellow}{startHour}{LogColors.Reset}");
                                break;
                            case "set":
                                var newTime = args.GetArgAs(3, startHour);

                                if (subCommand == "day")
                                {
                                    SetDayStartHour(newTime);
                                }
                                else
                                {
                                    SetNightStartHour(newTime);
                                }

                                Log.Information($"{subCommandCapitalized} start time set to {LogColors.Yellow}{newTime}{LogColors.Reset}");
                                break;
                        }

                        break;
                    case "ms":
                        var msAction = args.GetArgAs(2, "get");
                        var currentMs = subCommand == "day" ? Clock.GetDayMillisecondsPerGameMinute() : Clock.GetNightMillisecondsPerGameMinute();

                        switch (msAction)
                        {
                            case "get":
                                Log.Information($"{subCommandCapitalized} minute currently lasts {LogColors.Yellow}{currentMs}ms{LogColors.Reset}");
                                break;
                            case "set":
                                var newTime = args.GetArgAs(3, subCommand == "day" ? Clock.GetDayStartHour() : Clock.GetNightStartHour());

                                if (subCommand == "day")
                                {
                                    SetDayMillisecondsPerGameMinute(newTime);
                                }
                                else
                                {
                                    SetNightMillisecondsPerGameMinute(newTime);
                                }

                                Log.Information($"{subCommandCapitalized} minute set to last {LogColors.Yellow}{newTime}ms{LogColors.Reset}");
                                break;
                        }

                        break;
                }
            }
        }

        [EventHandler("onResourceStop")]
        private void OnResourceStop(string resourceName)
        {
            if (GetCurrentResourceName() != resourceName)
            {
                return;
            }

            Save();
        }

        private async Task UpdateTimeTick()
        {
            await Delay(Clock.GetCurrentMillisecondsPerGameMinute());

            _currentTime = _currentTime.AddMinutes(1);

            // Event format is time:onNext{item}, previous{item}, current{item}
            // e.g. time:onNextHour, previousHour, nextHour

            if (_currentTime.Year != _currentYear)
            {
                TriggerEvent("time:onNextYear", _currentYear, _currentTime.Year);
            }

            if (_currentTime.Month != _currentMonth)
            {
                TriggerEvent("time:onNextMonth", _currentMonth, _currentTime.Month);
            }

            if (_currentTime.Day != _currentDay)
            {
                TriggerEvent("time:onNextDay", _currentDay, _currentTime.Day);
            }

            if (_currentTime.Hour != _currentHour)
            {
                TriggerEvent("time:onNextHour", _currentHour, _currentTime.Hour);

                Save();
            }

            TriggerEvent("time:onNextMinute", _currentTime.Minute - 1, _currentTime.Minute);

            Sync();
        }
        #endregion
    }
}
