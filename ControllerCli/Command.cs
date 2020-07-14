using ControllerLib;
using ControllerLib.Configurations;
using System;

namespace ControllerCli
{
    public class Command
    {
        public Command(Configuration configuration)
        {
            _controller = new Controller(configuration);
            _gamepad = new Gamepad();
        }

        public Exception Exception { get; private set; }
        public Boolean OnException { get { return Exception != null; } }

        private Controller _controller { get; set; }
        private Gamepad _gamepad { get; set; }
        
        public void Execute()
        {          
            try
            {               
                _gamepad.Refresh(_controller.Synchronizer, _controller.Gamepad);

                _controller.Execute(View);
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                _gamepad.Clear();

                if (OnException)
                {
                    Console.WriteLine(Message.Exception(Exception.Message, Exception.InnerException?.Message));
                }

                Console.WriteLine(Message.PressAnyKeyToExit);
                Console.ReadLine();
            }        
        }

        private void View()
        {
            if (_controller.Gamepad.StateChanged)
            {
                _gamepad.Refresh(_controller.Synchronizer, _controller.Gamepad);
            }
        }
    }
}
