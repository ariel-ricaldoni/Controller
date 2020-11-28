using ControllerLib.Driver;
using System;

namespace ControllerLib.Input
{
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

    public class BatteryState : IEquatable<BatteryState>
    {
        public BatteryState(XINPUT_BATTERY_INFORMATION batteryInformation)
        {
            Type = Helper.EnumParse<BatteryType>(batteryInformation.BatteryType.ToString());
            Level = Helper.EnumParse<BatteryLevel>(batteryInformation.BatteryLevel.ToString());
        }

        public BatteryType Type { get; private set; }
        public BatteryLevel Level { get; private set; }

        public Boolean Equals(BatteryState other)
        {
            return this.Type == other?.Type && this.Level == other?.Level;
        }
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as BatteryState);
        }
        public override Int32 GetHashCode()
        {
            return this.Type.GetHashCode() ^ this.Level.GetHashCode();
        }
    }
}
