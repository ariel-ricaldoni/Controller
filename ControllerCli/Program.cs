using ControllerCli.View;
using System;

namespace ControllerCli
{
    public enum ExitCodes
    {
        Success = 0,
        StartupError = 1,
        ExecutionError = 2
    }

    public class Program
    {
        private static Int32 Main()
        {
            try
            {
                Console.Title = Message.ControllerNameAndVersion;

                var configuration = ConfigurationFactory.GetConfiguration();
                var view = ViewFactory.GetView(configuration.View, configuration.ResizeWindow);

                var command = new Command(configuration, view);
                command.Execute();

                return command.OnException ? (Int32)ExitCodes.ExecutionError : (Int32)ExitCodes.Success;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"{Message.AnErrorOccurred}{ex.Message}");

                Console.WriteLine(Message.PressAnyKeyToExit);
                Console.ReadLine();

                return (Int32)ExitCodes.StartupError;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Message.AnErrorOccurred}{ex}");

                Console.WriteLine(Message.PressAnyKeyToExit);
                Console.ReadLine();

                return (Int32)ExitCodes.ExecutionError;
            }
        }
    }
}
