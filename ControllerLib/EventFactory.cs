using ControllerLib.Input;
using ControllerLib.Output;
using System;

namespace ControllerLib
{
    public interface IEventFactory
    {
        InternalEventHandler GetInputLockEvent(KeyBindings keyBindings, Gamepad gamepad);
        InternalEventHandler GetApplyNextKeyBindingEvent(KeyBindings keyBindings, Gamepad gamepad, Action applyNextAction);
        InternalEventHandler GetApplyPreviousKeyBindingEvent(KeyBindings keyBindings, Gamepad gamepad, Action applyPreviousAction);
        AnalogEventHandler GetGamepadAnalogEvent(IAnalogInput input, KeyBindings keyBindings, Gamepad gamepad, Mouse mouse, Keyboard keyboard);
        ButtonEventHandler GetGamepadButtonEvent(IButtonInput input, KeyBindings keyBindings, Gamepad gamepad, Mouse mouse, Keyboard keyboard);
    }

    public class EventFactory : IEventFactory
    {
        public InternalEventHandler GetInputLockEvent(KeyBindings keyBindings, Gamepad gamepad)
        {
            return () =>
            {
                if (gamepad.InputState.ButtonPressCount == 2 && gamepad.InputState.BackPressed && gamepad.InputState.StartPressed)
                {
                    if (gamepad.KeyDown == 0)
                    {
                        gamepad.Vibrate(UInt16.MaxValue);
                        gamepad.DisableInput();

                        gamepad.AddKeyDown();
                    }
                    else if (gamepad.KeyDown < gamepad.KeyDownDelay)
                    {
                        gamepad.AddKeyDown();
                    }
                    else
                    {
                        gamepad.Vibrate(0);
                    }
                }
                else if (gamepad.PreviousInputState.ButtonPressCount == 2 && gamepad.PreviousInputState.BackPressed && gamepad.PreviousInputState.StartPressed)
                {
                    gamepad.Vibrate(0);

                    if (gamepad.KeyDown < gamepad.KeyDownDelay)
                    {
                        if (gamepad.PreviousIsEnabled) gamepad.EnableInput(); else gamepad.DisableInput();
                    }
                    else
                    {
                        if (gamepad.PreviousIsEnabled) gamepad.DisableInput(); else gamepad.EnableInput();
                    }

                    gamepad.ResetKeyDown();
                }
            };
        }
        public InternalEventHandler GetApplyNextKeyBindingEvent(KeyBindings keyBindings, Gamepad gamepad, Action applyNextAction)
        {
            return () =>
            {
                if (!gamepad.IsEnabled && gamepad.InputState.ButtonPressCount == 2
                    && gamepad.InputState.BackPressed && gamepad.PreviousInputState.BackPressed
                    && gamepad.InputState.RightPressed && !gamepad.PreviousInputState.RightPressed)
                {
                    var index = keyBindings.CurrentIndex < keyBindings.MaxIndex
                        ? keyBindings.CurrentIndex + 1
                        : keyBindings.MinIndex;

                    keyBindings.SetCurrent(index);
                    applyNextAction.Invoke();
                }
            };
        }
        public InternalEventHandler GetApplyPreviousKeyBindingEvent(KeyBindings keyBindings, Gamepad gamepad, Action applyPreviousAction)
        {
            return () =>
            {
                if (!gamepad.IsEnabled && gamepad.InputState.ButtonPressCount == 2
                    && gamepad.InputState.BackPressed && gamepad.PreviousInputState.BackPressed
                    && gamepad.InputState.LeftPressed && !gamepad.PreviousInputState.LeftPressed)
                {
                    var index = keyBindings.CurrentIndex > keyBindings.MinIndex
                        ? keyBindings.CurrentIndex - 1
                        : keyBindings.MaxIndex;

                    keyBindings.SetCurrent(index);
                    applyPreviousAction.Invoke();
                }
            };
        }
        public AnalogEventHandler GetGamepadAnalogEvent(IAnalogInput input, KeyBindings keyBindings, Gamepad gamepad, Mouse mouse, Keyboard keyboard)
        {
            if (input is CursorInput)
            {
                return CursorEventHandler(input as CursorInput, mouse);
            }
            else if (input is ScrollInput)
            {
                return ScrollEventHandler(input as ScrollInput, keyBindings, mouse);
            }
            else if (input is DirectionalKeysInput)
            {
                return DirectionalKeysEventHandler(input as DirectionalKeysInput, keyboard);
            }
            else
            {
                return null;
            }
        }
        public ButtonEventHandler GetGamepadButtonEvent(IButtonInput input, KeyBindings keyBindings, Gamepad gamepad, Mouse mouse, Keyboard keyboard)
        {
            if (input is MouseKeyInput)
            {
                return MouseEventHandler(input as MouseKeyInput, mouse);
            }
            else if (input is MouseMacroInput)
            {
                return MouseMacroEventHandler(input as MouseMacroInput);
            }
            else if (input is KeyboardKeyInput)
            {
                return KeyboardEventHandler(input as KeyboardKeyInput, keyboard);
            }
            else if (input is KeyboardMacroInput)
            {
                return KeyboardMacroEventHandler(input as KeyboardMacroInput, gamepad, keyboard);
            }
            else
            {
                return null;
            }
        }

        private AnalogEventHandler CursorEventHandler(CursorInput input, Mouse mouse)
        {
            return (stateX, stateY, previousStateX, previousStateY) =>
            {
                var speedX = mouse.GetCursorSpeed(stateX, Gamepad.MinAnalogLimit, Gamepad.MinAnalogDeadzone, Gamepad.MaxAnalogLimit, Gamepad.MaxAnalogDeadzone);
                var speedY = mouse.GetCursorSpeed(stateY, Gamepad.MinAnalogLimit, Gamepad.MinAnalogDeadzone, Gamepad.MaxAnalogLimit, Gamepad.MaxAnalogDeadzone);

                speedX = input.InvertX ? -speedX : speedX;
                speedY = input.InvertY ? speedY : -speedY;

                if (speedX != 0 || speedY != 0)
                {
                    mouse.Move(speedX, speedY);
                }
            };
        }
        private AnalogEventHandler ScrollEventHandler(ScrollInput input, KeyBindings keyBindings, Mouse mouse)
        {
            return (stateX, stateY, previousStateX, previousStateY) =>
            {
                var speed = mouse.GetCursorSpeed(stateY, Gamepad.MinAnalogLimit, Gamepad.MinAnalogDeadzone, Gamepad.MaxAnalogLimit, Gamepad.MaxAnalogDeadzone) * keyBindings.Current.ScrollSpeedMultiplier;

                speed = input.InvertY ? -speed : speed;

                if (speed != 0)
                {
                    mouse.Scroll((UInt32)speed);
                }
            };
        }
        private AnalogEventHandler DirectionalKeysEventHandler(DirectionalKeysInput input, Keyboard keyboard)
        {
            return (stateX, stateY, previousStateX, previousStateY) =>
            {
                if (stateX < Gamepad.MinAnalogDeadzone)
                {
                    keyboard.Press(input.LeftKey);
                }
                else if (stateX > Gamepad.MaxAnalogDeadzone)
                {
                    keyboard.Press(input.RightKey);
                }
                else if (previousStateX < Gamepad.MinAnalogDeadzone)
                {
                    keyboard.Release(input.LeftKey);
                }
                else if (previousStateX > Gamepad.MaxAnalogDeadzone)
                {
                    keyboard.Release(input.RightKey);
                }

                if (stateY < Gamepad.MinAnalogDeadzone)
                {
                    keyboard.Press(input.DownKey);
                }
                else if (stateY > Gamepad.MaxAnalogDeadzone)
                {
                    keyboard.Press(input.UpKey);
                }
                else if (previousStateY < Gamepad.MinAnalogDeadzone)
                {
                    keyboard.Release(input.DownKey);
                }
                else if (previousStateY > Gamepad.MaxAnalogDeadzone)
                {
                    keyboard.Release(input.UpKey);
                }
            };
        }
        private ButtonEventHandler MouseEventHandler(MouseKeyInput input, Mouse mouse)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (statePressed && !previousStatePressed)
                {
                    mouse.Press(input);
                }
                else if (!statePressed && previousStatePressed)
                {
                    mouse.Release(input);
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
        private ButtonEventHandler KeyboardEventHandler(KeyboardKeyInput input, Keyboard keyboard)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (statePressed && !previousStatePressed)
                {
                    keyboard.Press(input); keyboard.ResetKeyDown();
                }
                else if (statePressed && previousStatePressed)
                {
                    if (keyboard.KeyDown == keyboard.KeyDownDelay) keyboard.Press(input); else keyboard.AddKeyDown();
                }
                else if (!statePressed && previousStatePressed)
                {
                    keyboard.Release(input); keyboard.ResetKeyDown();
                }
            };
        }
        private ButtonEventHandler KeyboardMacroEventHandler(KeyboardMacroInput input, Gamepad gamepad, Keyboard keyboard)
        {
            switch (input.Macro)
            {
                case KeyboardMacro.OnScreenKeyboard:
                    return OnScreenKeyboardEventHandler(input, gamepad, keyboard);

                default:
                    return default;
            }
        }
        private ButtonEventHandler OnScreenKeyboardEventHandler(KeyboardMacroInput input, Gamepad gamepad, Keyboard keyboard)
        {
            return (statePressed, previousStatePressed) =>
            {
                if (!statePressed && previousStatePressed && gamepad.InputState.ButtonPressCount == 0 && gamepad.PreviousInputState.ButtonPressCount == 1)
                {
                    var windowsKey = new KeyboardKeyInput(KeyboardKeyCode.Windows);
                    var ctrlKey = new KeyboardKeyInput(KeyboardKeyCode.Ctrl);
                    var oKey = new KeyboardKeyInput(KeyboardKeyCode.O);

                    keyboard.Press(windowsKey);
                    keyboard.Press(ctrlKey);
                    keyboard.Press(oKey);

                    keyboard.Release(windowsKey);
                    keyboard.Release(ctrlKey);
                    keyboard.Release(oKey);
                }
            };
        }
    }
}
