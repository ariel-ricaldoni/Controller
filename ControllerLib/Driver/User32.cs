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
        UInt32 SendKeyboardInput(UInt16 key, UInt32 flag);
        UInt32 SendMouseInput(UInt32 flag, Int32 speedX, Int32 speedY);
        UInt32 SendMouseInput(UInt32 flag, UInt32 speed);
        UInt32 SendMouseInput(UInt32 flag);
    }

    public class User32 : IUser32
    {
        public UInt32 SendKeyboardInput(UInt16 key, UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Keyboard;
            input.Data.Keyboard.Vk = key;
            input.Data.Keyboard.Flags = flag;
            input.Data.Keyboard.Scan = (UInt16)(MapVirtualKey((UInt32)key, 0) & 0xFFU);
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;

            return SendInputs(new INPUT[] { input });
        }
        public UInt32 SendMouseInput(UInt32 flag, Int32 speedX, Int32 speedY)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.X = speedX;
            input.Data.Mouse.Y = speedY;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            return SendInputs(new INPUT[] { input });
        }
        public UInt32 SendMouseInput(UInt32 flag, UInt32 speed)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.MouseData = speed;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            return SendInputs(new INPUT[] { input });
        }
        public UInt32 SendMouseInput(UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            return SendInputs(new INPUT[] { input });
        }

        [DllImport("user32.dll")]
        private static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);
        [DllImport("user32.dll")]
        private static extern UInt32 MapVirtualKey(UInt32 uCode, UInt32 uMapType);

        private UInt32 SendInputs(INPUT[] inputs)
        {
            return SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
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
