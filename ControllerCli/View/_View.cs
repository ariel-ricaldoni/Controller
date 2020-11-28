using ControllerLib;
using ControllerLib.Input;
using System;

namespace ControllerCli.View
{
    public enum ViewType
    {
        Complete,
        Simple,
        None
    }

    public interface IView
    {
        public void Refresh(Controller state);
        public void Clear();
    }

    public static class ViewFactory
    {
        public static IView GetView(ViewType type)
        {
            switch (type)
            {
                case ViewType.Complete: return new Complete();

                case ViewType.Simple: return new Simple();

                case ViewType.None: return new None();

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public abstract class View : IView
    {
        protected String ModeName { get; set; }
        protected Int32 RefreshRate { get; set; }
        protected Boolean IsConnected { get; set; }
        protected Boolean IsEnabled { get; set; }
        protected InputState InputState { get; set; }
        protected BatteryState BatteryState { get; set; }

        protected String _buffer { get; set; }

        public virtual void Refresh(Controller state)
        {
            ReadControllerState(state);

            RefreshBuffer();
            Render();
        }
        public virtual void Clear()
        {
            ClearBuffer();
            Render();
        }

        protected virtual void ReadControllerState(Controller state)
        {
            RefreshRate = state.Synchronizer.RefreshRate;
            ModeName = state.KeyBindings.Current.Name;
            IsConnected = state.Gamepad.IsConnected;
            IsEnabled = state.Gamepad.IsEnabled;
            InputState = state.Gamepad.InputState;
            BatteryState = state.Gamepad.BatteryState;
        }
        protected void RefreshBuffer()
        {
            if (IsConnected)
            {
                WriteConnectedStateToBuffer();
            }
            else
            {
                WriteDisconnectedStateToBuffer();
            }
        }
        protected virtual void WriteConnectedStateToBuffer()
        {
            _buffer = $@"Xbox Controller connected.                                 
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
";
        }
        protected virtual void WriteDisconnectedStateToBuffer()
        {
            _buffer = $@"Xbox Controller disconnected.                              
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
";
        }

        protected void ClearBuffer()
        {
            _buffer = $@"                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
";
        }

        protected void Render()
        {
            Int32 currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(0, currentLineCursor);
            Console.WriteLine(_buffer);
            Console.SetCursorPosition(0, currentLineCursor);

            GC.Collect();
        }
    }
}
