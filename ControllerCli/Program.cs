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
                Console.SetWindowSize(60, 28);
                Console.SetBufferSize(60, 28);

                Console.WriteLine(Message.Header());

                var configuration = ConfigurationFactory.GetConfiguration();

                var command = new Command(configuration);
                command.Execute();

                return command.OnException ? (Int32)ExitCodes.ExecutionError : (Int32)ExitCodes.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine(Message.Exception(ex.Message, ex.InnerException?.Message));
                Console.ReadLine();

                return (Int32)ExitCodes.StartupError;
            }
        }
    }
}
