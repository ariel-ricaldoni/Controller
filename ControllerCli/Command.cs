using ControllerCli.View;
using ControllerLib;
using System;

namespace ControllerCli
{
    public class Command
    {
        public Command(Configuration configuration, IView view)
        {
            Configuration = configuration;
            View = view;
        }

        public Boolean OnException { get { return _exception != null; } }

        public void Execute()
        {
            try
            {
                ExecutionStarted();
            }
            catch (Exception ex)
            {
                ExecutionOnError(ex);
            }
            finally
            {
                ExecutionEnded();
            }
        }

        protected Configuration Configuration { get; private set; }
        protected IView View { get; private set; }

        private Exception _exception;

        private void ExecutionStarted()
        {
            Console.WriteLine($"{Message.ControllerNameAndVersion}{Environment.NewLine}");

            var controller = new Controller(Configuration.KeyBindings);

            View.Refresh(controller);

            controller.Invoke(() =>
            {
                if (controller.Gamepad.StateChanged)
                {
                    View.Refresh(controller);
                }
            });
        }
        private void ExecutionEnded()
        {
            View.Clear();

            if (OnException)
            {
                Console.WriteLine($"{Message.AnErrorOccurred}{_exception}");

                Console.WriteLine(Message.PressAnyKeyToExit);
                Console.ReadLine();
            }
        }
        private void ExecutionOnError(Exception ex)
        {
            _exception = ex;
        }
    }
}
