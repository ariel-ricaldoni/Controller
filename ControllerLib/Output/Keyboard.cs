using ControllerLib.Driver;
using ControllerLib.Input;
using System;

namespace ControllerLib.Output
{
    // https://docs.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
    public enum KeyboardKeyCode
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

        Num0 = 0x30,
        Num1 = 0x31,
        Num2 = 0x32,
        Num3 = 0x33,
        Num4 = 0x34,
        Num5 = 0x35,
        Num6 = 0x36,
        Num7 = 0x37,
        Num8 = 0x38,
        Num9 = 0x39,

        A = 0x41,
        B = 0x42,
        C = 0x43,
        D = 0x44,
        E = 0x45,
        F = 0x46,
        G = 0x47,
        H = 0x48,
        I = 0x49,
        J = 0x4A,
        K = 0x4B,
        L = 0x4C,
        M = 0x4D,
        N = 0x4E,
        O = 0x4F,
        P = 0x50,
        Q = 0x51,
        R = 0x52,
        S = 0x53,
        T = 0x54,
        U = 0x55,
        V = 0x56,
        W = 0x57,
        X = 0x58,
        Y = 0x59,
        Z = 0x5A,

        Windows = 0x5B,
        LeftWindows = 0x5B,
        RightWindows = 0x5C,
        Application = 0x5D,
        Sleep = 0x5F,

        NumPad0 = 0x60,
        NumPad1 = 0x61,
        NumPad2 = 0x62,
        NumPad3 = 0x63,
        NumPad4 = 0x64,
        NumPad5 = 0x65,
        NumPad6 = 0x66,
        NumPad7 = 0x67,
        NumPad8 = 0x68,
        NumPad9 = 0x69,

        Multiply = 0x6A,
        Add = 0x6B,
        Separator = 0x6C,
        Subtract = 0x6D,
        Decimal = 0x6E,
        Divide = 0x6F,

        F1 = 0x70,
        F2 = 0x71,
        F3 = 0x72,
        F4 = 0x73,
        F5 = 0x74,
        F6 = 0x75,
        F7 = 0x76,
        F8 = 0x77,
        F9 = 0x78,
        F10 = 0x79,
        F11 = 0x7A,
        F12 = 0x7B,
        F13 = 0x7C,
        F14 = 0x7D,
        F15 = 0x7E,
        F16 = 0x7F,
        F17 = 0x80,
        F18 = 0x81,
        F19 = 0x82,
        F20 = 0x83,
        F21 = 0x84,
        F22 = 0x85,
        F23 = 0x86,
        F24 = 0x87,

        NumLock = 0x90,
        ScrollLock = 0x91,
        LeftShift = 0xA0,
        RightShift = 0xA1,
        LeftCtrl = 0xA2,
        RightCtrl = 0xA3,
        LeftAlt = 0xA4,
        RightAlt = 0xA5,

        VolumeMute = 0xAD,
        VolumeDown = 0xAE,
        VolumeUp = 0xAF
    }
    public enum KeyboardKeyFlag
    {
        ExtendedKey = 0x0001,
        KeyDown = 0,
        KeyUp = 0x0002
    }
    public enum KeyboardMacro
    {
        OnScreenKeyboard
    }
    public enum ExtendedKeyCode
    {
        Ctrl = 0x11,
        LeftCtrl = 0xA2,
        RightCtrl = 0xA3,
        Ins = 0x2D,
        Del = 0x2E,
        End = 0x23,
        Home = 0x24,
        PageUp = 0x21,
        PageDown = 0x22,
        LeftArrow = 0x25,
        UpArrow = 0x26,
        RightArrow = 0x27,
        DownArrow = 0x28,
        NumLock = 0x90,
        PrintScreen = 0x2C,
        Divide = 0x6F,
    }

    public class Keyboard
    {
        public Keyboard()
        {
            _driver = new User32();
        }
        public Keyboard(IUser32 driver)
        {
            _driver = driver;
        }

        public const Int32 DefaultKeyDownDelay = 20;
        public const Int32 MinKeyDownDelayLimit = 0;
        public const Int32 MaxKeyDownDelayLimit = 144;

        public Int32 KeyDownDelay
        {
            get { return _keyDownDelay; }
            set { _keyDownDelay = (value < MinKeyDownDelayLimit ? MinKeyDownDelayLimit : (value > MaxKeyDownDelayLimit ? MaxKeyDownDelayLimit : value)); }
        }
        public Int32 KeyDown { get; private set; } = 0;

        private Int32 _keyDownDelay = DefaultKeyDownDelay;

        private IUser32 _driver { get; set; }

        public void Press(KeyboardKeyInput keyInput)
        {
            var key = (UInt16)keyInput.KeyCode;
            var flag = (UInt32)keyInput.KeyFlagDown;

            var input = new INPUT();
            input.Type = (Int32)InputType.Keyboard;
            input.Data.Keyboard.Vk = key;
            input.Data.Keyboard.Scan = (UInt16)(_driver.MapVirtualKeyWrapper((UInt32)key, 0) & 0xFFU);
            input.Data.Keyboard.Flags = flag;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }
        public void Release(KeyboardKeyInput keyInput)
        {
            var key = (UInt16)keyInput.KeyCode;
            var flag = (UInt32)keyInput.KeyFlagUp;

            var input = new INPUT();
            input.Type = (Int32)InputType.Keyboard;
            input.Data.Keyboard.Vk = key;
            input.Data.Keyboard.Scan = (UInt16)(_driver.MapVirtualKeyWrapper((UInt32)key, 0) & 0xFFU);
            input.Data.Keyboard.Flags = flag;
            input.Data.Keyboard.Time = 0;
            input.Data.Keyboard.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }

        public void AddKeyDown()
        {
            if (KeyDown < KeyDownDelay) KeyDown++;
        }
        public void ResetKeyDown()
        {
            KeyDown = 0;
        }

        public static KeyboardKeyFlag GetKeyFlagFromKeyCode(KeyboardKeyCode code, Boolean isDown)
        {
            var isExtendedKey = Enum.IsDefined(typeof(ExtendedKeyCode), (Int32)code);

            if (isDown)
            {
                return isExtendedKey ? KeyboardKeyFlag.ExtendedKey : KeyboardKeyFlag.KeyDown;
            }
            else
            {
                return isExtendedKey ? KeyboardKeyFlag.ExtendedKey | KeyboardKeyFlag.KeyUp : KeyboardKeyFlag.KeyUp;
            }
        }
    }

    public class KeyboardKeyInput : IButtonInput
    {
        public KeyboardKeyInput(KeyboardKeyCode keyCode)
        {
            KeyCode = (UInt16)keyCode;
            KeyFlagDown = (UInt32)Keyboard.GetKeyFlagFromKeyCode(keyCode, true);
            KeyFlagUp = (UInt32)Keyboard.GetKeyFlagFromKeyCode(keyCode, false);
        }

        public UInt16 KeyCode { get; set; }
        public UInt32 KeyFlagDown { get; set; }
        public UInt32 KeyFlagUp { get; set; }
    }
    public class KeyboardMacroInput : IButtonInput
    {
        public KeyboardMacroInput(KeyboardMacro macro)
        {
            Macro = macro;
        }

        public KeyboardMacro Macro { get; set; }
    }
    public class DirectionalKeysInput : IAnalogInput
    {
        public DirectionalKeysInput(KeyboardKeyCode upKeyCode, KeyboardKeyCode downKeyCode, KeyboardKeyCode leftKeyCode, KeyboardKeyCode rightKeyCode)
        {
            UpKey = new KeyboardKeyInput(upKeyCode);
            DownKey = new KeyboardKeyInput(downKeyCode);
            LeftKey = new KeyboardKeyInput(leftKeyCode);
            RightKey = new KeyboardKeyInput(rightKeyCode);
        }

        public KeyboardKeyInput UpKey { get; set; }
        public KeyboardKeyInput DownKey { get; set; }
        public KeyboardKeyInput LeftKey { get; set; }
        public KeyboardKeyInput RightKey { get; set; }
    }
}
