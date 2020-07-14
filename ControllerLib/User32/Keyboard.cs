using System;
using System.Runtime.InteropServices;

namespace ControllerLib.User32
{
    public class Keyboard : Driver
    {
        public Keyboard()
        {
            
        }

        public const Int32 DefaultKeyDownDelay = 20;

        public const Int32 KeyDownDelayMinLimit = 6;
        public const Int32 KeyDownDelayMaxLimit = 144;

        public enum KeyCode
        {
            Backspace = 0x08,
            Tab = 0x09,
            Clear = 0x0C,
            Enter = 0x0D,
            Shift = 0x10,
            Ctrl = 0x11,
            Alt = 0x12,
            Pause = 0x13,
            CapsLock = 0x14,
            Esc = 0x1B,
            Space = 0x20,
            PageUp = 0x21,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            LeftArrow = 0x25,
            UpArrow = 0x26,
            RightArrow = 0x27,
            DownArrow = 0x28,
            Select = 0x29,
            Print = 0x2A,
            Execute = 0x2B,
            PrintScreen = 0x2C,
            Ins = 0x2D,
            Del = 0x2E,
            Help = 0x2F,
            Windows = 0x5B
        }
        public enum KeyFlag
        {
            KeyDown = 0,
            KeyUp = 0x0002
        }
        public enum Macro
        {
            OnScreenKeyboard
        }

        public Int32 KeyDownDelay
        {
            get { return _keyDownDelay; }
            set { _keyDownDelay = (value < KeyDownDelayMinLimit ? KeyDownDelayMinLimit : (value > KeyDownDelayMaxLimit ? KeyDownDelayMaxLimit : value)); }
        } private Int32 _keyDownDelay = DefaultKeyDownDelay;
        public Int32 KeyDown { get; private set; } = 0;

        public void Press(UInt16 key)
        {
            SendInput(key, (UInt32)KeyFlag.KeyDown);
        }
        public void Release(UInt16 key)
        {
            SendInput(key, (UInt32)KeyFlag.KeyUp);
        }

        public void AddKeyDown()
        {
            if (KeyDown < KeyDownDelay) KeyDown++;
        }
        public void ResetKeyDown()
        {
            KeyDown = 0;
        }

        private void SendInput(UInt16 key, UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Keyboard;
            input.Data.Keyboard.Vk = key;
            input.Data.Keyboard.Flags = flag;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
