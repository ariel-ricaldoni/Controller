using ControllerLib.Configurations;
using ControllerLib.User32;
using ControllerLib.XInput;
using System;

namespace ControllerLib
{
    public class Controller
    {
        public Controller(Configuration configuration)
        {
            Configuration = configuration;

            Synchronizer = new Synchronizer();
            Synchronizer.TargetRefreshRate = Configuration.TargetRefreshRate;

            Mouse = new Mouse();
            Mouse.CursorMinSpeed = Configuration.CursorMinSpeed;
            Mouse.CursorMaxSpeed = Configuration.CursorMaxSpeed;

            Keyboard = new Keyboard();
            Keyboard.KeyDownDelay = Configuration.KeyDownDelay;

            Gamepad = new Gamepad();
            Gamepad.KeyDownDelay = Configuration.KeyDownDelay;

            Gamepad.InputLockHandler += GetInputLockEvent();
            
            Gamepad.LeftAnalogHandler += GetGamepadAnalogEvent(Configuration.LeftAnalog);
            Gamepad.LeftAnalogButtonHandler += GetGamepadButtonEvent(Configuration.LeftAnalogButton);
            Gamepad.RightAnalogHandler += GetGamepadAnalogEvent(Configuration.RightAnalog);
            Gamepad.RightAnalogButtonHandler += GetGamepadButtonEvent(Configuration.RightAnalogButton);
            
            Gamepad.UpButtonHandler += GetGamepadButtonEvent(Configuration.UpButton);
            Gamepad.DownButtonHandler += GetGamepadButtonEvent(Configuration.DownButton);
            Gamepad.LeftButtonHandler += GetGamepadButtonEvent(Configuration.LeftButton);
            Gamepad.RightButtonHandler += GetGamepadButtonEvent(Configuration.RightButton);
            
            Gamepad.AButtonHandler += GetGamepadButtonEvent(Configuration.AButton);
            Gamepad.BButtonHandler += GetGamepadButtonEvent(Configuration.BButton);
            Gamepad.XButtonHandler += GetGamepadButtonEvent(Configuration.XButton);
            Gamepad.YButtonHandler += GetGamepadButtonEvent(Configuration.YButton);
            
            Gamepad.LBButtonHandler += GetGamepadButtonEvent(Configuration.LBButton);
            Gamepad.LTButtonHandler += GetGamepadButtonEvent(Configuration.LTButton);
            Gamepad.RBButtonHandler += GetGamepadButtonEvent(Configuration.RBButton);
            Gamepad.RTButtonHandler += GetGamepadButtonEvent(Configuration.RTButton);
            
            Gamepad.StartButtonHandler += GetGamepadButtonEvent(Configuration.StartButton);
            Gamepad.BackButtonHandler += GetGamepadButtonEvent(Configuration.BackButton);      
        }

        public Configuration Configuration { get; private set; }

        public Synchronizer Synchronizer { get; private set; }
        public Mouse Mouse { get; private set; }
        public Keyboard Keyboard { get; private set; }
        public Gamepad Gamepad { get; private set; }
        
        public void Execute(Action view)
        {
            Synchronizer.While(() =>
            {
                Gamepad.ReadState();
                Mouse.ReadCursorPosition();

                view?.Invoke();

                Gamepad.HandleEvents();
            });
        }

        private InputLockEventHandler GetInputLockEvent()
        {
            return () =>
            {
                if (Gamepad.InputState.BackPressed && Gamepad.InputState.StartPressed && Gamepad.InputState.ButtonPressCount == 2)
                {
                    if (Gamepad.KeyDown == 0)
                    {
                        Gamepad.DisableInput();
                    }

                    if (Gamepad.KeyDown < Gamepad.KeyDownDelay)
                    {
                        Gamepad.Vibrate(UInt16.MaxValue); Gamepad.AddKeyDown();
                    }
                    else
                    {
                        Gamepad.Vibrate(0);
                    }
                }
                else if ((!Gamepad.InputState.BackPressed || !Gamepad.InputState.StartPressed) && Gamepad.InputState.ButtonPressCount != 2)
                {
                    if (Gamepad.KeyDown == Gamepad.KeyDownDelay)
                    {
                        Gamepad.ToggleInput();
                    }

                    Gamepad.Vibrate(0); Gamepad.ResetKeyDown();
                }
            };
        }

        private AnalogEventHandler GetGamepadAnalogEvent(GamepadAnalogInput input)
        {
            if (input is MouseCursorInput)
            {
                return CursorEventHandler(input as MouseCursorInput);
            }
            else if (input is MouseScrollInput)
            {
                return ScrollEventHandler(input as MouseScrollInput);
            }
            else
            {
                return null;
            }
        }     
        private AnalogEventHandler CursorEventHandler(MouseCursorInput input)
        {
            return (stateX, stateY) =>
            {
                var relativeX = Mouse.GetCursorSpeed(stateX, Gamepad.AnalogMaxValue, Gamepad.AnalogMaxDeadzone);
                var relativeY = Mouse.GetCursorSpeed(stateY, Gamepad.AnalogMaxValue, Gamepad.AnalogMaxDeadzone);

                if (input.InvertX) relativeX *= -1;
                if (input.InvertY == false) relativeY *= -1;

                if (relativeX != 0 || relativeY != 0)
                {
                    Mouse.Move(relativeX, relativeY);
                }
            };
        }  
        private AnalogEventHandler ScrollEventHandler(MouseScrollInput input)
        {
            return (stateX, stateY) =>
            {
                var relativeY = Mouse.GetCursorSpeed(stateY, Gamepad.AnalogMaxValue, Gamepad.AnalogMaxDeadzone) * Configuration.ScrollSpeedMultiplier;

                if (input.InvertY) relativeY *= -1;

                if (relativeY != 0)
                {
                    Mouse.Scroll((UInt32)relativeY);
                }
            };
        }
    
        private ButtonEventHandler GetGamepadButtonEvent(GamepadButtonInput input)
        {
            if (input is MouseClickInput)
            {
                return MouseEventHandler(input as MouseClickInput);
            }
            else if (input is MouseMacroInput)
            {
                return MouseMacroEventHandler(input as MouseMacroInput);
            }
            else if (input is KeyboardKeyInput)
            {
                return KeyboardEventHandler(input as KeyboardKeyInput);
            }
            else if (input is KeyboardMacroInput)
            {
                return KeyboardMacroEventHandler(input as KeyboardMacroInput);
            }
            else
            {
                return null;
            }
        }  
        private ButtonEventHandler MouseEventHandler(MouseClickInput input)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (statePressed && !previousStatePressed)
                {
                    Mouse.Click(input.KeyCodeDown);
                }
                else if (!statePressed && previousStatePressed)
                {
                    Mouse.Click(input.KeyCodeUp);
                }
            };
        }
        private ButtonEventHandler MouseMacroEventHandler(MouseMacroInput input)
        {
            switch (input.Macro)
            {
                default:
                    return default;
            }
        }
        private ButtonEventHandler KeyboardEventHandler(KeyboardKeyInput input)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (statePressed && !previousStatePressed)
                {
                    Keyboard.Press(input.KeyCode); Keyboard.ResetKeyDown();
                }
                else if (statePressed && previousStatePressed)
                {
                    if (Keyboard.KeyDown == Keyboard.KeyDownDelay) Keyboard.Press(input.KeyCode); else Keyboard.AddKeyDown();
                }
                else if (!statePressed && previousStatePressed)
                {
                    Keyboard.Release(input.KeyCode); Keyboard.ResetKeyDown();
                }
            };
        }
        private ButtonEventHandler KeyboardMacroEventHandler(KeyboardMacroInput input)
        {
            switch(input.Macro)
            {
                case Keyboard.Macro.OnScreenKeyboard: 
                    return OnScreenKeyboardEventHandler(input);

                default: 
                    return default;
            }
        }
        private ButtonEventHandler OnScreenKeyboardEventHandler(KeyboardMacroInput input)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (!statePressed && previousStatePressed && Gamepad.InputState.ButtonPressCount == 0 && Gamepad.PreviousInputState.ButtonPressCount == 1)
                {
                    Keyboard.Press((UInt16)Keyboard.KeyCode.Windows);
                    Keyboard.Press((UInt16)Keyboard.KeyCode.Ctrl);
                    Keyboard.Press(0x4F); //O Key

                    Keyboard.Release((UInt16)Keyboard.KeyCode.Windows);
                    Keyboard.Release((UInt16)Keyboard.KeyCode.Ctrl);
                    Keyboard.Release(0x4F); //O Key
                }
            };
        }
    }
}
