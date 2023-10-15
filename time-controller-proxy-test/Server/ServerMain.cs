using System;
using CitizenFX.Core;
using Magicallity.Time;

namespace time_controller_proxy_test.Server
{
    public class ServerMain : BaseScript
    {
        [Command("server_proxy_get", RemapParameters = true)]
        private void OnServerProxyGet()
        {
            Debug.WriteLine($"CurrentDate: {Clock.CurrentDate:G}");
            Debug.WriteLine($"CurrentDayTime: {Clock.CurrentDayTime:G}");

            Debug.WriteLine("");

            Debug.WriteLine($"MillisecondsPerGameMinute: {Clock.MillisecondsPerGameMinute}");
            Debug.WriteLine($"NightMillisecondsPerGameMinute: {Clock.NightMillisecondsPerGameMinute}");
            Debug.WriteLine($"DayMillisecondsPerGameMinute: {Clock.DayMillisecondsPerGameMinute}");

            Debug.WriteLine("");

            Debug.WriteLine($"DayStartHour: {Clock.DayStartHour}");
            Debug.WriteLine($"NightStartHour: {Clock.NightStartHour}");

            Debug.WriteLine("");

            Debug.WriteLine($"IsDay: {Clock.IsDay}");
            Debug.WriteLine($"IsNight: {Clock.IsNight}");

            Debug.WriteLine("");

            Debug.WriteLine($"Paused: {Clock.Paused}");
        }

        [Command("server_proxy_set_time", RemapParameters = true)]
        private void OnServerProxySetTime(string[] args)
        {
            var hour = args.GetArgAs(0, 0);
            var minute = args.GetArgAs(1, 0);
            var second = args.GetArgAs(2, 0);

            Debug.WriteLine($"Clock time is currently - {Clock.CurrentDayTime}");

            Clock.CurrentDayTime = new TimeSpan(hour, minute, second);

            Debug.WriteLine($"Clock time was set to - {Clock.CurrentDayTime}");
        }
    }
}