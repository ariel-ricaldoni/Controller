using ControllerCli.View;
using ControllerLib;
using System;
using System.Collections.Generic;

namespace ControllerCli
{
    public class Configuration
    {
        public ViewType View { get; set; } = ViewType.Complete;
        public Boolean ResizeWindow { get; set; } = true;

        public IList<KeyBinding> KeyBindings { get; set; } = new List<KeyBinding>();
    }
}
