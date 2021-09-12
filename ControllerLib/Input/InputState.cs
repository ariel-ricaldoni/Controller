using ControllerLib.Driver;
using System;

namespace ControllerLib.Input
{
    public enum GamepadKeyCode
    {
        Y = 32768,
        X = 16384,
        B = 8192,
        A = 4096,
        RB = 512,
        LB = 256,
        RightAnalogButton = 128,
        LeftAnalogButton = 64,
        Back = 32,
        Start = 16,
        Right = 8,
        Left = 4,
        Down = 2,
        Up = 1
    }

    public class InputState : IEquatable<InputState>
    {
        public InputState(XINPUT_STATE inputState)
        {
            PacketNumber = inputState.dwPacketNumber;

            LeftAnalogX = inputState.Gamepad.sThumbLX;
            LeftAnalogY = inputState.Gamepad.sThumbLY;
            RightAnalogX = inputState.Gamepad.sThumbRX;
            RightAnalogY = inputState.Gamepad.sThumbRY;

            LTPressed = inputState.Gamepad.bLeftTrigger;
            RTPressed = inputState.Gamepad.bRightTrigger;

            AssignButtons(inputState.Gamepad.wButtons);
        }

        public UInt32 PacketNumber { get; private set; } = 0;
        public Int16 ButtonPressCount { get; private set; } = 0;
        public Int16 LeftAnalogX { get; private set; } = 0;
        public Int16 LeftAnalogY { get; private set; } = 0;
        public Boolean LeftAnalogPressed { get; private set; } = false;
        public Int16 RightAnalogX { get; private set; } = 0;
        public Int16 RightAnalogY { get; private set; } = 0;
        public Boolean RightAnalogPressed { get; private set; } = false;
        public Boolean UpPressed { get; private set; } = false;
        public Boolean DownPressed { get; private set; } = false;
        public Boolean LeftPressed { get; private set; } = false;
        public Boolean RightPressed { get; private set; } = false;
        public Boolean APressed { get; private set; } = false;
        public Boolean BPressed { get; private set; } = false;
        public Boolean XPressed { get; private set; } = false;
        public Boolean YPressed { get; private set; } = false;
        public Boolean LBPressed { get; private set; } = false;
        public byte LTPressed { get; private set; } = 0;
        public Boolean RBPressed { get; private set; } = false;
        public byte RTPressed { get; private set; } = 0;
        public Boolean StartPressed { get; private set; } = false;
        public Boolean BackPressed { get; private set; } = false;

        public Boolean Equals(InputState other)
        {
            return this.PacketNumber == other?.PacketNumber;
        }
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as InputState);
        }
        public override Int32 GetHashCode()
        {
            return this.PacketNumber.GetHashCode();
        }

        private void AssignButtons(UInt16 wButtons)
        {
            ButtonPressCount = 0;

            while (wButtons > 0)
            {
                ButtonPressCount++;

                if (wButtons >= (UInt16)GamepadKeyCode.Y)
                {
                    YPressed = true; wButtons -= (UInt16)GamepadKeyCode.Y;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.X)
                {
                    XPressed = true; wButtons -= (UInt16)GamepadKeyCode.X;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.B)
                {
                    BPressed = true; wButtons -= (UInt16)GamepadKeyCode.B;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.A)
                {
                    APressed = true; wButtons -= (UInt16)GamepadKeyCode.A;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.RB)
                {
                    RBPressed = true; wButtons -= (UInt16)GamepadKeyCode.RB;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.LB)
                {
                    LBPressed = true; wButtons -= (UInt16)GamepadKeyCode.LB;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.RightAnalogButton)
                {
                    RightAnalogPressed = true; wButtons -= (UInt16)GamepadKeyCode.RightAnalogButton;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.LeftAnalogButton)
                {
                    LeftAnalogPressed = true; wButtons -= (UInt16)GamepadKeyCode.LeftAnalogButton;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Back)
                {
                    BackPressed = true; wButtons -= (UInt16)GamepadKeyCode.Back;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Start)
                {
                    StartPressed = true; wButtons -= (UInt16)GamepadKeyCode.Start;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Right)
                {
                    RightPressed = true; wButtons -= (UInt16)GamepadKeyCode.Right;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Left)
                {
                    LeftPressed = true; wButtons -= (UInt16)GamepadKeyCode.Left;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Down)
                {
                    DownPressed = true; wButtons -= (UInt16)GamepadKeyCode.Down;
                }
                else if (wButtons >= (UInt16)GamepadKeyCode.Up)
                {
                    UpPressed = true; wButtons -= (UInt16)GamepadKeyCode.Up;
                }
            }
        }
    }
}
