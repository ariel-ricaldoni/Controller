using ControllerLib.User32;
using System;

namespace ControllerLib.Configurations
{
    public class MouseCursorInput : GamepadAnalogInput
    {
        public MouseCursorInput() : base()
        {

        }
        public MouseCursorInput(Boolean invertX, Boolean invertY) : this()
        {
            InvertX = invertX;
            InvertY = invertY;
        }

        public Boolean InvertX { get; set; } = false;
        public Boolean InvertY { get; set; } = false;
    }
    public class MouseScrollInput : GamepadAnalogInput
    {
        public MouseScrollInput() : base()
        {

        }
        public MouseScrollInput(Boolean invertY) : this()
        {
            InvertY = invertY;
        }

        public Boolean InvertY { get; set; } = false;
    }
    public class MouseClickInput : GamepadButtonInput
    {
        public MouseClickInput() : base()
        {

        }
        public MouseClickInput(Mouse.KeyCode keyCode) : this()
        {
            KeyCodeDown = (UInt32)Mouse.GetKeyFlagFromKeyCode(keyCode, true);
            KeyCodeUp = (UInt32)Mouse.GetKeyFlagFromKeyCode(keyCode, false);
        }

        public UInt32 KeyCodeDown { get; set; }
        public UInt32 KeyCodeUp { get; set; }
    }
    public class MouseMacroInput : GamepadButtonInput
    {
        public MouseMacroInput() : base()
        {

        }
        public MouseMacroInput(Mouse.Macro macro) : base()
        {
            Macro = macro;
        }

        public Mouse.Macro Macro { get; set; }
    }
}
