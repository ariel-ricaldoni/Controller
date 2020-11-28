using System;
using System.Runtime.InteropServices;

namespace ControllerLib.Driver
{
    public enum InputType
    {
        Mouse = 0,
        Keyboard = 1
    }

    public interface IUser32
    {
        Int32 GetMarshalSizeOf(Type type);

        UInt32 SendInputWrapper(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);
        UInt32 MapVirtualKeyWrapper(UInt32 uCode, UInt32 uMapType);
    }

    public class User32 : Driver, IUser32
    {
        public UInt32 SendInputWrapper(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize)
        {
            return SendInput(nInputs, pInputs, cbSize);
        }

        public UInt32 MapVirtualKeyWrapper(UInt32 uCode, UInt32 uMapType)
        {
            return MapVirtualKey(uCode, uMapType);
        }


        [DllImport("user32.dll")]
        public static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);

        [DllImport("user32.dll")]
        public static extern UInt32 MapVirtualKey(UInt32 uCode, UInt32 uMapType);
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        public UInt32 Type;
        public MOUSEKEYBDINPUT Data;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct MOUSEKEYBDINPUT
    {
        [FieldOffset(0)]
        public MOUSEINPUT Mouse;
        [FieldOffset(0)]
        public KEYBDINPUT Keyboard;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public Int32 X;
        public Int32 Y;
        public UInt32 MouseData;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public UInt16 Vk;
        public UInt16 Scan;
        public UInt32 Flags;
        public UInt32 Time;
        public IntPtr ExtraInfo;
    }
}
