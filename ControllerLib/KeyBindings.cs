using ControllerLib.Input;
using ControllerLib.Output;
using System;
using System.Collections.Generic;

namespace ControllerLib
{
    public class KeyBindings
    {
        public KeyBindings()
        {
            _keyBindings = new List<KeyBinding> { new KeyBinding() };
        }
        public KeyBindings(IList<KeyBinding> keyBindings)
        {
            _keyBindings = keyBindings;
        }

        public KeyBinding Current { get { return GetCurrent(); } }
        public Int32 CurrentIndex { get; private set; } = 0;
        public Int32 MinIndex { get { return 0; } }
        public Int32 MaxIndex { get { return _keyBindings.Count - 1; } }
        public Int32 Count { get { return _keyBindings.Count; } }

        public KeyBinding GetCurrent()
        {
            return _keyBindings[CurrentIndex];
        }
        public void SetCurrent(Int32 index)
        {
            if (index < MinIndex || index > MaxIndex) throw new ArgumentOutOfRangeException("KeyBindings index is out of range.");

            CurrentIndex = index;
        }

        private IList<KeyBinding> _keyBindings { get; set; }
    }

    public class KeyBinding
    {
        public const String DefaultName = "Windows";

        public String Name { get; set; } = DefaultName;

        public Int32 TargetRefreshRate { get; set; } = Synchronizer.DefaultTargetRefreshRate;
        public Int32 CursorMaxSpeed { get; set; } = Mouse.DefaultMaxCursorSpeed;
        public Int32 ScrollSpeedMultiplier { get; set; } = Mouse.DefaultScrollSpeedMultiplier;
        public Int32 KeyDownDelay { get; set; } = Keyboard.DefaultKeyDownDelay;

        public IAnalogInput LeftAnalog { get; set; } = new CursorInput();
        public IButtonInput LeftAnalogButton { get; set; } = new MouseKeyInput(MouseKeyCode.LeftMouseButton);
        public IAnalogInput RightAnalog { get; set; } = new ScrollInput();
        public IButtonInput RightAnalogButton { get; set; } = new MouseKeyInput(MouseKeyCode.MiddleMouseButton);

        public IButtonInput UpButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.UpArrow);
        public IButtonInput DownButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.DownArrow);
        public IButtonInput LeftButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.LeftArrow);
        public IButtonInput RightButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.RightArrow);

        public IButtonInput AButton { get; set; } = new MouseKeyInput(MouseKeyCode.LeftMouseButton);
        public IButtonInput BButton { get; set; } = new MouseKeyInput(MouseKeyCode.RightMouseButton);
        public IButtonInput XButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Enter);
        public IButtonInput YButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Backspace);

        public IButtonInput LBButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Tab);
        public IButtonInput LTButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Ctrl);
        public IButtonInput RBButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Shift);
        public IButtonInput RTButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Alt);

        public IButtonInput StartButton { get; set; } = new KeyboardKeyInput(KeyboardKeyCode.Windows);
        public IButtonInput BackButton { get; set; } = new KeyboardMacroInput(KeyboardMacro.OnScreenKeyboard);
    }
}
