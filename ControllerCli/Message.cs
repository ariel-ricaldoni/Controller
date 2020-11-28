using System;
using System.Diagnostics;
using System.Reflection;

namespace ControllerCli
{
    internal static class Message
    {
        public static FileVersionInfo FileVersionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);

        public static String ControllerNameAndVersion = $"Controller CLI v{FileVersionInfo.FileVersion}";

        public static String PressAnyKeyToExit = $"Press any key to exit...";

        public static String AnErrorOccurred = "An error occurred: ";
    }
}
