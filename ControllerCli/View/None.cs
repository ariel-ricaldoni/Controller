using System;

namespace ControllerCli.View
{
    public class None : View
    {
        public None(Boolean resizeWindow = false)
        {
            if (resizeWindow)
            {
                try { Console.CursorVisible = false; Console.SetWindowSize(60, 4); Console.SetBufferSize(60, 4); } catch { }
            }
        }
    }
}
