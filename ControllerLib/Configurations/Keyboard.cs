using ControllerLib.User32;
using System;

namespace ControllerLib.Configurations
{
    public class KeyboardKeyInput : GamepadButtonInput
    {
        public KeyboardKeyInput() : base()
        {

        }
        public KeyboardKeyInput(Keyboard.KeyCode keyCode) : this()
        {
            KeyCode = (UInt16)keyCode;
        }

        public UInt16 KeyCode { get; set; }
    }
    public class KeyboardMacroInput : GamepadButtonInput
    {
        public KeyboardMacroInput() : base()
        {

        }
        public KeyboardMacroInput(Keyboard.Macro macro) : base()
        {
            Macro = macro;
        }

        public Keyboard.Macro Macro { get; set; }
    }
}
