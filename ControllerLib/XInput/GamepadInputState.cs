using System;

namespace ControllerLib.XInput
{
    public class GamepadInputState : Driver, IEquatable<GamepadInputState>
    {
        public GamepadInputState(XINPUT_STATE inputState)
        {
            PacketNumber = inputState.dwPacketNumber;

            LeftAnalogX = inputState.Gamepad.sThumbLX;
            LeftAnalogY = inputState.Gamepad.sThumbLY;
            RightAnalogX = inputState.Gamepad.sThumbRX;
            RightAnalogY = inputState.Gamepad.sThumbRY;

            LTPressed = inputState.Gamepad.bLeftTrigger;
            RTPressed = inputState.Gamepad.bRightTrigger;

            ButtonPressCount = 0;

            AssignButtons(inputState.Gamepad.wButtons);
        }

        public enum Button
        {
            Y = 32768,
            X = 16384,
            B = 8192,
            A = 4096,
            RB = 512,
            LB = 256,
            RightAnalog = 128,
            LeftAnalog = 64,
            Back = 32,
            Start = 16,
            Right = 8,
            Left = 4,
            Down = 2,
            Up = 1
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
     
        public Boolean Equals(GamepadInputState other)
        {
            return this.PacketNumber == other?.PacketNumber;
        }
        public override Boolean Equals(Object obj)
        {
            return this.Equals(obj as GamepadInputState);
        }
        public override Int32 GetHashCode()
        {
            return this.PacketNumber.GetHashCode();
        }

        private void AssignButtons(UInt16 wButtons)
        {
            while (wButtons > 0)
            {
                ButtonPressCount++;

                if (wButtons >= (UInt16)Button.Y)
                {
                    YPressed = true; wButtons -= (UInt16)Button.Y;
                }
                else if (wButtons >= (UInt16)Button.X)
                {
                    XPressed = true; wButtons -= (UInt16)Button.X;
                }
                else if (wButtons >= (UInt16)Button.B)
                {
                    BPressed = true; wButtons -= (UInt16)Button.B;
                }
                else if (wButtons >= (UInt16)Button.A)
                {
                    APressed = true; wButtons -= (UInt16)Button.A;
                }
                else if (wButtons >= (UInt16)Button.RB)
                {
                    RBPressed = true; wButtons -= (UInt16)Button.RB;
                }
                else if (wButtons >= (UInt16)Button.LB)
                {
                    LBPressed = true; wButtons -= (UInt16)Button.LB;
                }
                else if (wButtons >= (UInt16)Button.RightAnalog)
                {
                    RightAnalogPressed = true; wButtons -= (UInt16)Button.RightAnalog;
                }
                else if (wButtons >= (UInt16)Button.LeftAnalog)
                {
                    LeftAnalogPressed = true; wButtons -= (UInt16)Button.LeftAnalog;
                }
                else if (wButtons >= (UInt16)Button.Back)
                {
                    BackPressed = true; wButtons -= (UInt16)Button.Back;
                }
                else if (wButtons >= (UInt16)Button.Start)
                {
                    StartPressed = true; wButtons -= (UInt16)Button.Start;
                }
                else if (wButtons >= (UInt16)Button.Right)
                {
                    RightPressed = true; wButtons -= (UInt16)Button.Right;
                }
                else if (wButtons >= (UInt16)Button.Left)
                {
                    LeftPressed = true; wButtons -= (UInt16)Button.Left;
                }
                else if (wButtons >= (UInt16)Button.Down)
                {
                    DownPressed = true; wButtons -= (UInt16)Button.Down;
                }
                else if (wButtons >= (UInt16)Button.Up)
                {
                    UpPressed = true; wButtons -= (UInt16)Button.Up;
                }
            }
        }
    }
}
