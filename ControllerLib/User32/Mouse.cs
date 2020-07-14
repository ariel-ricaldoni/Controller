using System;
using System.Runtime.InteropServices;

namespace ControllerLib.User32
{
    public class Mouse : Driver
    {
        public Mouse()
        {
            
        }

        public const Int32 DefaultCursorMinSpeed = 1;
        public const Int32 DefaultCursorMaxSpeed = 16;
        public const Int32 DefaultScrollSpeedMultiplier = 3;

        public const Int32 CursorSpeedMinLimit = 1;
        public const Int32 CursorSpeedMaxLimit = 60;

        public const Int32 ScrollSpeedMultiplierMinLimit = 1;
        public const Int32 ScrollSpeedMultiplierMaxLimit = 10;

        public enum KeyCode
        {
            LeftMouseButton = 0x0002,
            RightMouseButton = 0x0008
        }
        public enum KeyFlag
        {
            LeftButtonDown = 0x0002,
            LeftButtonUp = 0x0004,
            RightButtonDown = 0x0008,
            RightButtonUp = 0x0010,
            WheelMoved = 0x0800
        }
        public enum Macro
        {
            
        }

        public Int32 CursorMinSpeed
        {
            get { return _cursorMinSpeed; }
            set { _cursorMinSpeed = (value < CursorSpeedMinLimit ? CursorSpeedMinLimit : (value > _cursorMaxSpeed ? _cursorMaxSpeed : value)); }
        } private Int32 _cursorMinSpeed = DefaultCursorMinSpeed;
        public Int32 CursorMaxSpeed
        {
            get { return _cursorMaxSpeed; }
            set { _cursorMaxSpeed = (value < _cursorMinSpeed ? _cursorMinSpeed : (value > CursorSpeedMaxLimit ? CursorSpeedMaxLimit : value)); }
        } private Int32 _cursorMaxSpeed = DefaultCursorMaxSpeed;
        public Int32 ScrollSpeedMultiplier
        {
            get { return _scrollSpeedMultiplier; }
            set { _scrollSpeedMultiplier = (value < ScrollSpeedMultiplierMinLimit ? ScrollSpeedMultiplierMinLimit : (value > ScrollSpeedMultiplierMaxLimit ? ScrollSpeedMultiplierMaxLimit : value)); }
        } private Int32 _scrollSpeedMultiplier = 2;

        public Int32 CursorX { get; private set; } = 0;
        public Int32 CursorY { get; private set; } = 0;

        public void ReadCursorPosition()
        {
            var point = new POINT();
            GetCursorPos(out point);

            CursorX = point.x;
            CursorY = point.y;
        }
        public Int32 GetCursorSpeed(Int32 input, Int32 maxInput, Int32 inputDeadZone)
        {
            var inputIsPositive = input > 0;

            input = Math.Abs(input);

            if (input > inputDeadZone)
            {
                Int32 cursormaxInput = maxInput - inputDeadZone;
                Int32 positiveGamepadAnalogInput = input - inputDeadZone;

                var Speed = positiveGamepadAnalogInput * CursorMaxSpeed / cursormaxInput;

                if (Speed < CursorMinSpeed) Speed = CursorMinSpeed;
                else if (Speed > CursorMaxSpeed) Speed = CursorMaxSpeed;

                return inputIsPositive ? Speed : Speed * -1;
            }
            else
            {
                return 0;
            }
        }

        public void Move(Int32 relativeX, Int32 relativeY)
        {
            var newX = CursorX + relativeX;
            var newY = CursorY + relativeY;

            SetCursorPos(newX, newY);
        }
        public void Scroll(UInt32 speed)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = (UInt32)KeyFlag.WheelMoved;
            input.Data.Mouse.MouseData = speed;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }
        public void Click(UInt32 flag)
        {
            var input = new INPUT();
            input.Type = (Int32)InputType.Mouse;
            input.Data.Mouse.Flags = flag;

            var inputs = new INPUT[] { input };
            SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        public static KeyFlag GetKeyFlagFromKeyCode(KeyCode code, Boolean buttonDown = true)
        {
            switch (code)
            {
                case KeyCode.LeftMouseButton:
                    return buttonDown ? KeyFlag.LeftButtonDown : KeyFlag.LeftButtonUp;

                case Mouse.KeyCode.RightMouseButton:
                    return buttonDown ? KeyFlag.RightButtonDown : KeyFlag.RightButtonUp;

                default:
                    return default;
            }
        }
    }
}
