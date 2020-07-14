using ControllerLib.User32;
using System;

namespace ControllerLib.Configurations
{
    public class Configuration
    {
        public Int32 CursorMinSpeed { get; set; } = Mouse.DefaultCursorMinSpeed;
        public Int32 CursorMaxSpeed { get; set; } = Mouse.DefaultCursorMaxSpeed;
        public Int32 ScrollSpeedMultiplier { get; set; } = Mouse.DefaultScrollSpeedMultiplier;
        public Int32 KeyDownDelay { get; set; } = Keyboard.DefaultKeyDownDelay;
        public Int32 TargetRefreshRate { get; set; } = Synchronizer.DefaultTargetRefreshRate;

        public GamepadAnalogInput LeftAnalog { get; set; } = new MouseCursorInput();
        public GamepadAnalogInput RightAnalog { get; set; } = new MouseScrollInput();

        public GamepadButtonInput LeftAnalogButton { get; set; } = new MouseClickInput(Mouse.KeyCode.LeftMouseButton);
        public GamepadButtonInput RightAnalogButton { get; set; } = new MouseClickInput();

        public GamepadButtonInput UpButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.UpArrow);
        public GamepadButtonInput DownButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.DownArrow);
        public GamepadButtonInput LeftButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.LeftArrow);
        public GamepadButtonInput RightButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.RightArrow);

        public GamepadButtonInput AButton { get; set; } = new MouseClickInput(Mouse.KeyCode.LeftMouseButton);
        public GamepadButtonInput BButton { get; set; } = new MouseClickInput(Mouse.KeyCode.RightMouseButton);
        public GamepadButtonInput XButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Enter);
        public GamepadButtonInput YButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Backspace);

        public GamepadButtonInput LBButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Tab);
        public GamepadButtonInput LTButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Ctrl);
        public GamepadButtonInput RBButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Shift);
        public GamepadButtonInput RTButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Alt);

        public GamepadButtonInput StartButton { get; set; } = new KeyboardKeyInput(Keyboard.KeyCode.Windows);
        public GamepadButtonInput BackButton { get; set; } = new KeyboardMacroInput(Keyboard.Macro.OnScreenKeyboard);
    }
}
