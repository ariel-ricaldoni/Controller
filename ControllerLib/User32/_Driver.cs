using System;
using System.Runtime.InteropServices;

namespace ControllerLib.User32
{
    public abstract class Driver
    {
        protected enum InputType
        {
            Mouse = 0,
            Keyboard = 1
        }

        protected void SendMouseClickInput(UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        protected void SendMouseScrollInput(UInt32 flag, UInt32 speed)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.MouseData = speed;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        protected void SendKeyboardKeyInput(UInt16 key, UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Keyboard;
            input.Data.Keyboard.Vk = key;
            input.Data.Keyboard.Flags = flag;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));           
        }

        [DllImport("user32.dll")]
        protected static extern Boolean GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        protected static extern Boolean SetCursorPos(Int32 x, Int32 y);

        [DllImport("user32.dll")]
        protected static extern UInt32 SendInput(UInt32 nInputs, INPUT[] pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Explicit)]
        public struct POINT
        {
            [FieldOffset(0)]
            public Int32 x;

            [FieldOffset(4)]
            public Int32 y;
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
            public MouseClickInput Mouse;
            [FieldOffset(0)]
            public KEYBDINPUT Keyboard;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MouseClickInput
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
            public Int32 ExtraInfo;
        }
    }
}
