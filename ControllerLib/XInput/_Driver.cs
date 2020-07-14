using System;
using System.Runtime.InteropServices;

namespace ControllerLib.XInput
{
    public abstract class Driver
    {
        protected void Vibrate(UInt32 dwUserIndex, UInt16 leftMotorSpeed, UInt16 rightMotorSpeed)
        {
            var inputVibration = new XINPUT_VIBRATION();
            inputVibration.wLeftMotorSpeed = leftMotorSpeed;
            inputVibration.wRightMotorSpeed = rightMotorSpeed;

            XInputSetState(dwUserIndex, ref inputVibration);
        }

        [DllImport("xinput1_4.dll")]
        protected static extern UInt32 XInputGetState(UInt32 dwUserIndex, ref XINPUT_STATE pState);

        [DllImport("xinput1_4.dll")]
        protected static extern UInt32 XInputSetState(UInt32 dwUserIndex, ref XINPUT_VIBRATION pVibration);

        [DllImport("xinput1_4.dll")]
        protected static extern UInt32 XInputGetCapabilities(UInt32 dwUserIndex, UInt32 dwFlags, ref XINPUT_CAPABILITIES pCapabilities);

        [DllImport("xinput1_4.dll")]
        protected static extern UInt32 XInputGetBatteryInformation(UInt32 dwUserIndex, Byte devType, ref XINPUT_BATTERY_INFORMATION pBatteryInformation);

        [DllImport("xinput1_4.dll")]
        protected static extern UInt32 XInputGetKeystroke(UInt32 dwUserIndex, UInt32 dwReserved, ref XINPUT_KEYSTROKE pKeystroke);

        [DllImport("xinput1_4.dll")]
        protected static extern void XInputEnable(Boolean enable);

        [DllImport("xinput1_4.dll)")]
        protected static extern UInt32 XInputGetAudioDeviceIds(UInt32 dwUserIndex, [MarshalAs(UnmanagedType.LPWStr)]out String pRenderDeviceId, ref UInt32 pRenderCount, [MarshalAs(UnmanagedType.LPWStr)]out String pCaptureDeviceId, ref UInt32 pCaptureCount);

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
        public struct XINPUT_CAPABILITIES
        {
            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(0)]
            public Byte Type;

            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(1)]
            public Byte SubType;

            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(2)]
            public UInt16 Flags;

            [FieldOffset(4)]
            public XINPUT_GAMEPAD Gamepad;

            [FieldOffset(16)]
            public XINPUT_VIBRATION Vibration;
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

        [StructLayout(LayoutKind.Explicit)]
        public struct XINPUT_KEYSTROKE
        {
            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(0)]
            public UInt16 VirtualKey;

            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(2)]
            public char Unicode;

            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(4)]
            public UInt16 Flags;

            [MarshalAs(UnmanagedType.I2)]
            [FieldOffset(5)]
            public Byte UserIndex;

            [MarshalAs(UnmanagedType.I1)]
            [FieldOffset(6)]
            public Byte HidCode;
        }
    }
}
