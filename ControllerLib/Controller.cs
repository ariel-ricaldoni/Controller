using ControllerLib.Driver;
using ControllerLib.Input;
using ControllerLib.Output;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ControllerLib
{
    public class Controller
    {
        public Controller()
        {
            EventFactory = new EventFactory();

            KeyBindings = new KeyBindings();
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad();
            Mouse = new Mouse();
            Keyboard = new Keyboard();
        }
        public Controller(IList<KeyBinding> keyBindings)
        {
            EventFactory = new EventFactory();

            KeyBindings = new KeyBindings(keyBindings);
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad();
            Mouse = new Mouse();
            Keyboard = new Keyboard();
        }
        public Controller(IEventFactory eventFactory)
        {
            EventFactory = eventFactory;

            KeyBindings = new KeyBindings();
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad();
            Mouse = new Mouse();
            Keyboard = new Keyboard();
        }
        public Controller(IEventFactory eventFactory, IList<KeyBinding> keyBindings)
        {
            EventFactory = eventFactory;

            KeyBindings = new KeyBindings(keyBindings);
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad();
            Mouse = new Mouse();
            Keyboard = new Keyboard();
        }
        public Controller(IEventFactory eventFactory, IXInput xInput, IUser32 user32)
        {
            EventFactory = eventFactory;

            KeyBindings = new KeyBindings();
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad(xInput);
            Mouse = new Mouse(user32);
            Keyboard = new Keyboard(user32);
        }
        public Controller(IEventFactory eventFactory, IXInput xInput, IUser32 user32, IList<KeyBinding> keyBindings)
        {
            EventFactory = eventFactory;

            KeyBindings = new KeyBindings(keyBindings);
            Synchronizer = new Synchronizer();

            Gamepad = new Gamepad(xInput);
            Mouse = new Mouse(user32);
            Keyboard = new Keyboard(user32);
        }

        public IEventFactory EventFactory { get; private set; }
        public KeyBindings KeyBindings { get; private set; }
        public Synchronizer Synchronizer { get; private set; }
        public Gamepad Gamepad { get; private set; }
        public Mouse Mouse { get; private set; }
        public Keyboard Keyboard { get; private set; }
        
        public void Invoke(Action view)
        {
            ApplyCurrentKeyBinding();

            Synchronizer.While(() =>
            {
                Gamepad.ReadState();

                view.Invoke();

                Gamepad.HandleEvents();
            });
        }
        public void Invoke(Action view, CancellationToken cancellationToken)
        {
            ApplyCurrentKeyBinding();

            Synchronizer.While(() =>
            {
                Gamepad.ReadState();

                view.Invoke();

                Gamepad.HandleEvents();

                if (cancellationToken.IsCancellationRequested)
                {
                    Synchronizer.Stop();
                }
            });
        }
        public void ApplyCurrentKeyBinding()
        {
            var keyBinding = KeyBindings.GetCurrent();

            ApplyProperties(keyBinding);
            ApplyEvents(keyBinding);
        }
    
        private void ApplyProperties(KeyBinding keyBinding)
        {
            Synchronizer.TargetRefreshRate = keyBinding.TargetRefreshRate;

            Gamepad.KeyDownDelay = keyBinding.KeyDownDelay;
            
            Mouse.CursorMaxSpeed = keyBinding.CursorMaxSpeed;
            Mouse.ScrollSpeedMultiplier = keyBinding.ScrollSpeedMultiplier;
            
            Keyboard.KeyDownDelay = keyBinding.KeyDownDelay;                    
        }
        private void ApplyEvents(KeyBinding keyBinding)
        {
            Gamepad.SystemHandler = EventFactory.GetInputLockEvent(KeyBindings, Gamepad);

            if (KeyBindings.Count > 1)
            {
                Gamepad.SystemHandler += EventFactory.GetApplyNextKeyBindingEvent(KeyBindings, Gamepad, () => ApplyCurrentKeyBinding());
                Gamepad.SystemHandler += EventFactory.GetApplyPreviousKeyBindingEvent(KeyBindings, Gamepad, () => ApplyCurrentKeyBinding());
            }

            Gamepad.LeftAnalogHandler = EventFactory.GetGamepadAnalogEvent(keyBinding.LeftAnalog, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.LeftAnalogButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.LeftAnalogButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.RightAnalogHandler = EventFactory.GetGamepadAnalogEvent(keyBinding.RightAnalog, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.RightAnalogButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.RightAnalogButton, KeyBindings, Gamepad, Mouse, Keyboard);

            Gamepad.UpButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.UpButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.DownButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.DownButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.LeftButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.LeftButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.RightButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.RightButton, KeyBindings, Gamepad, Mouse, Keyboard);

            Gamepad.AButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.AButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.BButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.BButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.XButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.XButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.YButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.YButton, KeyBindings, Gamepad, Mouse, Keyboard);

            Gamepad.LBButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.LBButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.LTButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.LTButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.RBButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.RBButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.RTButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.RTButton, KeyBindings, Gamepad, Mouse, Keyboard);

            Gamepad.StartButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.StartButton, KeyBindings, Gamepad, Mouse, Keyboard);
            Gamepad.BackButtonHandler = EventFactory.GetGamepadButtonEvent(keyBinding.BackButton, KeyBindings, Gamepad, Mouse, Keyboard);
        }
    }
}
