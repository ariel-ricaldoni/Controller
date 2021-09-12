﻿using ControllerCli.View;
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
                Console.WriteLine($"{Message.ControllerNameAndVersion}{Environment.NewLine}");

                TryConfigureConsoleWindow();
         
                var configuration = ConfigurationFactory.GetConfiguration();
                var view = ViewFactory.GetView(configuration.View);

                var command = new Command(configuration, view);
                command.Execute();

                return command.OnException ? (Int32)ExitCodes.ExecutionError : (Int32)ExitCodes.Success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{Message.AnErrorOccurred}{ex}");
                Console.ReadLine();

                return (Int32)ExitCodes.StartupError;
            }
        }

        private static void TryConfigureConsoleWindow()
        {
            try
            {
                Console.CursorVisible = false;
                Console.SetWindowSize(60, 28);
                Console.SetBufferSize(60, 28);
            }
            catch
            {

            }
        }
    }
}
