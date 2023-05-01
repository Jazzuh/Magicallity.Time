using System;
using CfxUtils.Shared.Convar;
using CfxUtils.Shared.Logging;
using CitizenFX.Core;
using Magicallity.Time.Shared;
using static CitizenFX.Core.Native.API;

namespace Magicallity.Time.Client
{
    public class TimeController : BaseScript
    {
        #region Fields
        private bool _manualTimeControl;

        private ReplicatedConvar<bool> _syncTimeOnSpawnConvar = new ReplicatedConvar<bool>("clock_sync_time_on_spawn", true);
        #endregion
        
        #region Constructor
        public TimeController()
        {
            AddStateBagChangeHandler("clock:ticks", "global", new Action<string, string, dynamic, int, bool>((bagName, key, value, res, replicated) =>
            {
                Log.Trace($"Clock has been updated - {LogColors.Yellow}{value}{LogColors.Reset}");

                if (GetManualTimeControl())
                {
                    return;
                }

                var time = new DateTime(long.Parse(value));
                    
                SetClockDate(time.Day, time.Month, time.Year);
                NetworkOverrideClockTime(time.Hour, time.Minute, time.Second);
            }));

            AddStateBagChangeHandler("clock:paused", "global", new Action<string, string, dynamic, int, bool>((bagName, key, value, res, replicated) =>
            {
                Log.Trace($"Clock paused state has been updated - {LogColors.Yellow}{value}{LogColors.Reset}");

                if (GetManualTimeControl())
                {
                    return;
                }

                if (value)
                {
                    SetMillisecondsPerGameMinute(int.MaxValue);
                }
                else
                {
                    Sync();
                }
            }));

            Exports.Add("Sync", new Action(Sync));
            Exports.Add("SetManualTimeControl", new Action<bool>(SetManualTimeControl));
            Exports.Add("GetManualTimeControl", new Func<bool>(GetManualTimeControl));
        }
        #endregion

        #region Methods
        /// <summary>
        /// Syncs the clock with the global state
        /// </summary>
        public void Sync()
        {
            if (_manualTimeControl)
            {
                return;
            }

            SetMillisecondsPerGameMinute(Clock.GetPaused() ? int.MaxValue : Clock.GetCurrentMillisecondsPerGameMinute());

            SetClockDate(Clock.GetDayOfMonth(), Clock.GetClockMonth(), Clock.GetClockYear());
            NetworkOverrideClockTime(Clock.GetHours(), Clock.GetMinutes(), Clock.GetSeconds());
        }

        /// <summary>
        /// Sets if the clock will sync with the server. If set to true, client side scripts will have to manually control the clock
        /// </summary>
        /// <param name="state">If the clock will be manually controlled or not</param>
        public void SetManualTimeControl(bool state)
        {
            _manualTimeControl = state;

            if (_manualTimeControl)
            {
                SetMillisecondsPerGameMinute(int.MaxValue);
            }
            else
            {
                Sync();
            }
        }

        /// <summary>
        /// Gets if the clock is being manually controlled
        /// </summary>
        /// <returns>If the clock is being manually controlled</returns>
        public bool GetManualTimeControl()
        {
            return _manualTimeControl;
        }
        #endregion

        #region Commands, Events and Ticks
        [EventHandler("playerSpawned")]
        private void OnPlayerSpawned()
        {
            if (!_syncTimeOnSpawnConvar.Value)
            {
                return;
            }

            Sync();
        }
        #endregion
    }
}
