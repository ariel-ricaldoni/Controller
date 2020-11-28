using ControllerLib.Driver;
using ControllerLib.Input;
using System;

namespace ControllerLib.Output
{
    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-mouse_event
    public enum MouseKeyCode
    {
        LeftMouseButton = 0x0002,
        RightMouseButton = 0x0008,
        MiddleMouseButton = 0x0020
    }
    public enum MouseKeyFlag
    {
        LeftButtonDown = 0x0002,
        LeftButtonUp = 0x0004,
        RightButtonDown = 0x0008,
        RightButtonUp = 0x0010,
        MiddleButtonDown = 0x0008,
        MiddleButtonUp = 0x0040,

        MouseMoved = 0x0001,
        WheelMoved = 0x0800
    }
    public enum MouseMacro
    {

    }

    public class Mouse
    {
        public Mouse()
        {
            _driver = new User32();
        }
        public Mouse(IUser32 driver)
        {
            _driver = driver;
        }

        public const Int32 DefaultMaxCursorSpeed = 16;
        public const Int32 DefaultScrollSpeedMultiplier = 3;
        public const Int32 MinCursorSpeedLimit = 1;
        public const Int32 MaxCursorSpeedLimit = 60;
        public const Int32 MinScrollSpeedMultiplierLimit = 1;
        public const Int32 MaxScrollSpeedMultiplierLimit = 10;

        public Int32 CursorMaxSpeed
        {
            get { return _cursorMaxSpeed; }
            set { _cursorMaxSpeed = (value < MinCursorSpeedLimit ? MinCursorSpeedLimit : value > MaxCursorSpeedLimit ? MaxCursorSpeedLimit : value); }
        }
        public Int32 ScrollSpeedMultiplier
        {
            get { return _scrollSpeedMultiplier; }
            set { _scrollSpeedMultiplier = (value < MinScrollSpeedMultiplierLimit ? MinScrollSpeedMultiplierLimit : (value > MaxScrollSpeedMultiplierLimit ? MaxScrollSpeedMultiplierLimit : value)); }
        }

        private Int32 _cursorMaxSpeed = DefaultMaxCursorSpeed;
        private Int32 _scrollSpeedMultiplier = DefaultScrollSpeedMultiplier;

        private IUser32 _driver;

        public void Move(Int32 speedX, Int32 speedY)
        {
            var flag = (UInt32)MouseKeyFlag.MouseMoved;

            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.X = speedX;
            input.Data.Mouse.Y = speedY;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }
        public void Scroll(UInt32 speed)
        {
            var flag = (UInt32)MouseKeyFlag.WheelMoved;

            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.MouseData = speed;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }
        public void Press(MouseKeyInput keyInput)
        {
            var flag = (UInt32)keyInput.KeyFlagDown;

            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }
        public void Release(MouseKeyInput keyInput)
        {
            var flag = (UInt32)keyInput.KeyFlagUp;

            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;
            input.Data.Mouse.Time = 0;
            input.Data.Mouse.ExtraInfo = IntPtr.Zero;

            var inputs = new INPUT[] { input };
            _driver.SendInputWrapper((UInt32)inputs.Length, inputs, _driver.GetMarshalSizeOf(typeof(INPUT)));
        }

        public Int32 GetCursorSpeed(Int32 value, Int32 minLimit, Int32 minDeadZone, Int32 maxLimit, Int32 maxDeadZone)
        {
            if (value > maxDeadZone)
            {
                return CalculateCursorSpeed(value, maxLimit, maxDeadZone, CursorMaxSpeed);
            }
            else if (value < minDeadZone)
            {
                return CalculateCursorSpeed(value, minLimit, minDeadZone, -CursorMaxSpeed);
            }
            else
            {
                return 0;
            }
        }

        public static MouseKeyFlag GetKeyFlagFromKeyCode(MouseKeyCode code, Boolean isDown)
        {
            switch (code)
            {
                case MouseKeyCode.LeftMouseButton:
                    return isDown ? MouseKeyFlag.LeftButtonDown : MouseKeyFlag.LeftButtonUp;

                case MouseKeyCode.RightMouseButton:
                    return isDown ? MouseKeyFlag.RightButtonDown : MouseKeyFlag.RightButtonUp;

                case MouseKeyCode.MiddleMouseButton:
                    return isDown ? MouseKeyFlag.MiddleButtonDown : MouseKeyFlag.MiddleButtonUp;

                default:
                    return default;
            }
        }

        private Int32 CalculateCursorSpeed(Int32 value, Int32 limit, Int32 deadZone, Int32 speedLimit)
        {
            return (value - deadZone) * speedLimit / (limit - deadZone);
        }
    }

    public class MouseKeyInput : IButtonInput
    {
        public MouseKeyInput(MouseKeyCode keyCode)
        {
            KeyCode = (UInt32)keyCode;
            KeyFlagDown = (UInt32)Mouse.GetKeyFlagFromKeyCode(keyCode, true);
            KeyFlagUp = (UInt32)Mouse.GetKeyFlagFromKeyCode(keyCode, false);
        }

        public UInt32 KeyCode { get; set; }
        public UInt32 KeyFlagDown { get; set; }
        public UInt32 KeyFlagUp { get; set; }
    }
    public class MouseMacroInput : IButtonInput
    {
        public MouseMacroInput(MouseMacro macro)
        {
            Macro = macro;
        }

        public MouseMacro Macro { get; set; }
    }
    public class CursorInput : IAnalogInput
    {
        public CursorInput()
        {

        }
        public CursorInput(Boolean invertX, Boolean invertY)
        {
            InvertX = invertX;
            InvertY = invertY;
        }

        public Boolean InvertX { get; set; } = false;
        public Boolean InvertY { get; set; } = false;
    }
    public class ScrollInput : IAnalogInput
    {
        public ScrollInput()
        {

        }
        public ScrollInput(Boolean invertY)
        {
            InvertY = invertY;
        }

        public Boolean InvertY { get; set; } = false;
    }
}
