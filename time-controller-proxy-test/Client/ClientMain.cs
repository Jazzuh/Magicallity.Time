using System;
using CitizenFX.Core;
using Magicallity.Time;

namespace time_controller_proxy_test.Client
{
    public class ClientMain : BaseScript
    {
        [Command("client_proxy_get")]
        private void OnClientProxyGet(string[] args)
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

            Debug.WriteLine($"ManuallyControl: {Clock.ManuallyControl}");
            Debug.WriteLine($"Paused: {Clock.Paused}");
        }

        [Command("client_proxy_set_control")]
        private void OnClientProxySetControl(string[] args)
        {
            var manuallyControl = args.GetArgAs(0, false);

            Clock.ManuallyControl = manuallyControl;

            Debug.WriteLine($"Clock manual control state is: {Clock.ManuallyControl}");
        }

        [Command("client_proxy_set_time")]
        private void OnClientProxySetTime(string[] args)
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