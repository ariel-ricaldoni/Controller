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

            _view = view;
        }

        public Boolean OnException { get { return _exception != null; } }
        public Configuration Configuration { get; private set; }

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

        private IView _view { get; set; }
        private Exception _exception;
   
        private void ExecutionStarted()
        {
            var controller = new Controller(Configuration.KeyBindings);

            _view.Refresh(controller);

            controller.Invoke(() =>
            {
                if (controller.Gamepad.StateChanged)
                {
                    _view.Refresh(controller);
                }
            });
        }
        private void ExecutionEnded()
        {
            _view.Clear();

            if (OnException)
            {
                Console.WriteLine($"{Message.AnErrorOccurred}{_exception.ToString()}");

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
