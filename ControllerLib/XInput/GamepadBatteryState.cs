using System;

namespace ControllerLib.XInput
{   
    public class GamepadBatteryState : Driver, IEquatable<GamepadBatteryState>
    {     
        public GamepadBatteryState(XINPUT_BATTERY_INFORMATION batteryInformation)
        {
            Type = Helper.EnumParse<BatteryType>(batteryInformation.BatteryType.ToString());
            Level = Helper.EnumParse<BatteryLevel>(batteryInformation.BatteryLevel.ToString());
        }

        public enum BatteryType
        {
            Disconnected = 0,
            Wired = 1,
            Alkaline = 2,
            Nimh = 3,
            Unknown = 4
        }
        public enum BatteryLevel
        {
            Empty = 0,
            Low = 1,
            Medium = 2,
            Full = 3
        }

        public BatteryType Type { get; private set; }
        public BatteryLevel Level { get; private set; }

        public Boolean Equals(GamepadBatteryState other)
        {
            return this.Type == other?.Type && this.Level == other?.Level;
        }
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as GamepadBatteryState);
        }
        public override Int32 GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Level.GetHashCode();
        }
    }
}
