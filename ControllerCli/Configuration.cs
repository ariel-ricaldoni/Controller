using ControllerCli.View;
using ControllerLib;
using System.Collections.Generic;

namespace ControllerCli
{
    public class Configuration
    {
        public ViewType View { get; set; } = ViewType.Complete;

        public IList<KeyBinding> KeyBindings { get; set; } = new List<KeyBinding>();
    }
}
