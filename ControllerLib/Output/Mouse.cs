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

        public void Move(Int32 speedX, Int32 speedY)
        {
            _driver.SendMouseInput((UInt32)MouseKeyFlag.MouseMoved, speedX, speedY);
        }
        public void Scroll(UInt32 speed)
        {
            _driver.SendMouseInput((UInt32)MouseKeyFlag.WheelMoved, speed);
        }
        public void Press(MouseKeyInput keyInput)
        {
            _driver.SendMouseInput((UInt32)keyInput.KeyFlagDown);
        }
        public void Release(MouseKeyInput keyInput)
        {
            _driver.SendMouseInput((UInt32)keyInput.KeyFlagUp);
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

        private Int32 _cursorMaxSpeed = DefaultMaxCursorSpeed;
        private Int32 _scrollSpeedMultiplier = DefaultScrollSpeedMultiplier;
        private IUser32 _driver;

        private Int32 CalculateCursorSpeed(Int32 value, Int32 limit, Int32 deadZone, Int32 maxSpeed)
        {
            return (value - deadZone) * maxSpeed / (limit - deadZone);
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
