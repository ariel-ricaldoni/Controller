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

        public static String Header()
        {
            return $"{ControllerNameAndVersion}{Environment.NewLine}";
        }
        public static String Exception(String exception, String innerException)
        {
            exception = exception.Replace("An error occurred: ", String.Empty).Replace(Environment.NewLine, String.Empty);
            innerException = innerException?.Replace(Environment.NewLine, String.Empty);

            return $"An error occurred: {exception} {innerException}{Environment.NewLine}";
        }

        public static String InvalidConfigurationFile(Int32 line)
        {
            return $"Invalid configuration file. Could not read line {line}.";
        }
        public static String InvalidConfigurationFile(Int32 line, String key, String value)
        {
            return $"Invalid configuration file. Could not read value \"{value}\" on parameter \"{key}\" on line {line}.";
        }
    }
}
