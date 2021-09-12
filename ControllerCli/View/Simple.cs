using ControllerLib.Input;
using System;

namespace ControllerCli.View
{
    public class Simple : View
    {
        protected override void WriteConnectedState()
        {
            var BATTERY = String.Empty;
            switch (BatteryState.Level)
            {
                case BatteryLevel.Empty: BATTERY = "000%"; break;
                case BatteryLevel.Low: BATTERY = "033%"; break;
                case BatteryLevel.Medium: BATTERY = "066%"; break;
                case BatteryLevel.Full: BATTERY = "100%"; break;
            }

            var FPS = RefreshRate.ToString("D3");
            var INPUT_LOCK = IsEnabled ? "OFF" : "ON ";
            var MODE = (ModeName.Length > 3 ? ModeName.Substring(0, 3) : ModeName.PadLeft(3, ' ')).ToUpper();

            _view = $@"Xbox Controller connected.                                 
                                                           
 ■■■■■■ FPS: {FPS} | BAT: {BATTERY} | LOCK: {INPUT_LOCK} | MODE: {MODE} ■■■■■■
";
        }
        protected override void WriteDisconnectedState()
        {
            _view = $@"Xbox Controller disconnected.                              
                                                           
                                                           
";
        }
        protected override void ClearView()
        {
            _view = $@"                                                           
                                                           
                                                           
";
        }
    }
}
