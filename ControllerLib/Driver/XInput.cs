using System;
using System.Runtime.InteropServices;

namespace ControllerLib.Driver
{
    public interface IXInput
    {
        Int32 GetMarshalSizeOf(Type type);

        UInt32 GetState(UInt32 dwUserIndex, ref XINPUT_STATE pState);
        UInt32 SetState(UInt32 dwUserIndex, ref XINPUT_VIBRATION pVibration);
        UInt32 GetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);
    }

    public class XInput : Driver, IXInput
    {
        public UInt32 GetState(UInt32 dwUserIndex, ref XINPUT_STATE pState)
        {
            return XInputGetState(dwUserIndex, ref pState);
        }

        public UInt32 SetState(UInt32 dwUserIndex, ref XINPUT_VIBRATION pVibration)
        {
            return XInputSetState(dwUserIndex, ref pVibration);
        }

        public UInt32 GetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation)
        {
            return XInputGetBatteryInformation(dwUserIndex, devType, ref pBatteryInformation);
        }


        [DllImport("xinput1_4.dll")]
        public static extern UInt32 XInputGetState(UInt32 dwUserIndex, ref XINPUT_STATE pState);

        [DllImport("xinput1_4.dll")]
        public static extern UInt32 XInputSetState(UInt32 dwUserIndex, ref XINPUT_VIBRATION pVibration);

        [DllImport("xinput1_4.dll")]
        public static extern UInt32 XInputGetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct XINPUT_STATE
    {
        [FieldOffset(0)]
        public UInt32 dwPacketNumber;

        [FieldOffset(4)]
        public XINPUT_GAMEPAD Gamepad;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct XINPUT_GAMEPAD
    {
        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(0)]
        public UInt16 wButtons;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(2)]
        public Byte bLeftTrigger;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(3)]
        public Byte bRightTrigger;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(4)]
        public Int16 sThumbLX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(6)]
        public Int16 sThumbLY;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(8)]
        public Int16 sThumbRX;

        [MarshalAs(UnmanagedType.I2)]
        [FieldOffset(10)]
        public Int16 sThumbRY;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct XINPUT_VIBRATION
    {
        [MarshalAs(UnmanagedType.I2)]
        public UInt16 wLeftMotorSpeed;

        [MarshalAs(UnmanagedType.I2)]
        public UInt16 wRightMotorSpeed;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct XINPUT_BATTERY_INFORMATION
    {
        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(0)]
        public Byte BatteryType;

        [MarshalAs(UnmanagedType.I1)]
        [FieldOffset(1)]
        public Byte BatteryLevel;
    }
}
