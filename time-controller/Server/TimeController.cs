using System;
using System.Linq;
using CfxUtils.Convar.Shared;
using CfxUtils.Logging;
using CitizenFX.Core;
using Magicallity.Time.Shared;
using static CitizenFX.Server.Native.Natives;

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
            StateBag.Global["clock:day:msPerMinute"] = _msPerMinuteDuringDayConvar.Value;
            StateBag.Global["clock:day:start"] = _dayStartHourConvar.Value;

            StateBag.Global["clock:night:msPerMinute"] = _msPerMinuteDuringNightConvar.Value;
            StateBag.Global["clock:night:start"] = _nightHourStartConvar.Value;

            StateBag.Global["clock:paused"] = _isClockPausedConvar.Value;

            var timeTickString = GetResourceKvpString("clock:ticks").ToString() ?? "0";
            var timeTick = long.Parse(timeTickString);

            _currentTime = timeTick == 0 ? DateTime.UtcNow : new DateTime(timeTick, DateTimeKind.Utc);

            Sync();

            if (!Clock.GetPaused())
            {
                Tick += UpdateTimeTick;
            }
        }
        #endregion

        #region Sync and Save
        /// <summary>
        /// Syncs the clock state to all clients
        /// </summary>
        public void Sync()
        {
            Debug.WriteLine($"Syncing clock state to all clients");

            StateBag.Global["clock:ticks"] = _currentTime.Ticks.ToString();

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
            Debug.WriteLine($"Saving clock state as {LogFormatter.ToCyan(_currentTime.ToString("G"))}");

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
        [Export("SetDayStartHour")]
        public void SetDayStartHour(int newHour)
        {
            newHour = MathUtil.Clamp(newHour, 0, 23);

            StateBag.Global["clock:day:start"] = newHour;
            _dayStartHourConvar.Value = newHour;
        }

        /// <summary>
        /// Gets the hour which indicates the start of the night
        /// </summary>
        /// <param name="newHour">The hour which marks the start of the night</param>
        [Export("SetNightStartHour")]
        public void SetNightStartHour(int newHour)
        {
            newHour = MathUtil.Clamp(newHour, 0, 23);

            StateBag.Global["clock:night:start"] = newHour;
            _nightHourStartConvar.Value = newHour;
        }
        #endregion

        #region Minute
        /// <summary>
        /// Sets the amount of milliseconds that a game minute will take during the day
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds a game minute during the day will take</param>
        [Export("SetDayMillisecondsPerGameMinute")]
        public void SetDayMillisecondsPerGameMinute(int milliseconds)
        {
            StateBag.Global["clock:day:msPerMinute"] = milliseconds;
            _msPerMinuteDuringDayConvar.Value = milliseconds;
        }

        /// <summary>
        /// Sets the amount of milliseconds that a game minute will take during the night
        /// </summary>
        /// <param name="milliseconds">The amount of milliseconds a game minute during the day will take</param>
        [Export("SetNightMillisecondsPerGameMinute")]
        public void SetNightMillisecondsPerGameMinute(int milliseconds)
        {
            StateBag.Global["clock:night:msPerMinute"] = milliseconds;
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
        [Export("SetClockTime")]
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
        [Export("SetClockDate")]
        public void SetClockDate(int day, int month, int year)
        {
            _currentTime = new DateTime(year, month, day, _currentTime.Hour, _currentTime.Minute, _currentTime.Second);

            SaveAndSync();
        }

        /// <summary>
        /// Sets the clock pause state
        /// </summary>
        /// <param name="state">The state of the clock pause</param>
        [Export("PauseClock")]
        public void PauseClock(bool state)
        {
            StateBag.Global["clock:paused"] = state;

            _isClockPausedConvar.Value = state;

            Tick -= UpdateTimeTick;

            if (!state)
            {
                Tick += UpdateTimeTick;
            }
        }
        #endregion

        #region Commands, Events and Ticks
        [Command("time", Restricted = true, RemapParameters = true)]
        [Command("clock", Restricted = true, RemapParameters = true)]
        private void DoEditCommand(string[] args)
        {
            var subCommand = args.GetArgAs(0, "").ToLower();

            if (subCommand is "set")
            {
                SetClockTime(args.GetArgAs(1, 0), args.GetArgAs(2, 0), args.GetArgAs(3, 0));

                Debug.WriteLine($"Set clock to: {LogFormatter.ToYellow($"{Clock.GetHours():D2}:{Clock.GetMinutes():D2}")}");
            }
            else if (subCommand is "get")
            {
                Debug.WriteLine($"Current clock is: {LogFormatter.ToYellow($"{Clock.GetHours():D2}:{Clock.GetMinutes():D2}")}");
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

                Debug.WriteLine($"Clock has been {(Clock.GetPaused() ? "paused".ToYellow() : "unpaused".ToGreen())}");
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
                                Debug.WriteLine($"{subCommandCapitalized} currently starts at {startHour.ToString().ToYellow()}");
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

                                Debug.WriteLine($"{subCommandCapitalized} start time set to {newTime.ToString().ToYellow()}");
                                break;
                        }
                            
                        break;
                    case "ms":
                        var msAction = args.GetArgAs(2, "get");
                        var currentMs = subCommand == "day" ? Clock.GetDayMillisecondsPerGameMinute() : Clock.GetNightMillisecondsPerGameMinute();

                        switch (msAction)
                        {
                            case "get":
                                Debug.WriteLine($"{subCommandCapitalized} minute currently lasts {LogFormatter.ToYellow(currentMs + "ms")}");
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

                                Debug.WriteLine($"{subCommandCapitalized} minute set to last {LogFormatter.ToYellow(currentMs + "ms")}");
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

        private async Coroutine UpdateTimeTick()
        {
            await Delay((uint)Clock.GetCurrentMillisecondsPerGameMinute());

            if (Clock.GetPaused())
            {
                return;
            }

            _currentTime = _currentTime.AddMinutes(1);

            // Event format is time:onNext{item}, previous{item}, current{item}
            // e.g. time:onNextHour, previousHour, nextHour

            if (_currentTime.Year != _currentYear)
            {
                Events.TriggerEvent("time:onNextYear", _currentYear, _currentTime.Year);
            }

            if (_currentTime.Month != _currentMonth)
            {
                Events.TriggerEvent("time:onNextMonth", _currentMonth, _currentTime.Month);
            }

            if (_currentTime.Day != _currentDay)
            {
                Events.TriggerEvent("time:onNextDay", _currentDay, _currentTime.Day);
            }

            if (_currentTime.Hour != _currentHour)
            {
                Events.TriggerEvent("time:onNextHour", _currentHour, _currentTime.Hour);

                Save();
            }

            Events.TriggerEvent("time:onNextMinute", _currentTime.Minute - 1, _currentTime.Minute);

            Sync();
        }
        #endregion
    }
}
