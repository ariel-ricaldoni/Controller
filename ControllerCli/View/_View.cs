using ControllerLib;
using ControllerLib.Input;
using System;

namespace ControllerCli.View
{
    public enum ViewType
    {
        Complete,
        Basic,
        None
    }

    public interface IView
    {
        public void Refresh(Controller state);
        public void Clear();
    }

    public static class ViewFactory
    {
        public static IView GetView(ViewType type, Boolean resizeWindow)
        {
            switch (type)
            {
                case ViewType.Complete: return new Complete(resizeWindow);

                case ViewType.Basic: return new Basic(resizeWindow);

                case ViewType.None: return new None(resizeWindow);

                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

    public abstract class View : IView
    {
        public virtual void Refresh(Controller state)
        {
            ReadControllerState(state);

            RefreshView();
            Render();
        }
        public virtual void Clear()
        {
            ClearView();
            Render();
        }

        protected String ModeName { get; set; }
        protected Int32 RefreshRate { get; set; }
        protected Boolean IsConnected { get; set; }
        protected Boolean IsEnabled { get; set; }
        protected InputState InputState { get; set; }
        protected BatteryState BatteryState { get; set; }
        protected String _view { get; set; }

        protected virtual void ReadControllerState(Controller state)
        {
            RefreshRate = state.Synchronizer.RefreshRate;
            ModeName = state.KeyBindings.Current.Name;
            IsConnected = state.Gamepad.IsConnected;
            IsEnabled = state.Gamepad.IsEnabled;
            InputState = state.Gamepad.InputState;
            BatteryState = state.Gamepad.BatteryState;
        }
        protected virtual void WriteConnectedState()
        {
            _view = $@"Xbox Controller connected.                                 ";
        }
        protected virtual void WriteDisconnectedState()
        {
            _view = $@"Xbox Controller disconnected.                              ";
        }
        protected virtual void ClearView()
        {
            _view = $@"                                                           ";
        }
        protected void RefreshView()
        {
            if (IsConnected)
            {
                WriteConnectedState();
            }
            else
            {
                WriteDisconnectedState();
            }
        }
        protected void Render()
        {
            Int32 currentLineCursor = Console.CursorTop;

            Console.SetCursorPosition(0, currentLineCursor);
            Console.WriteLine(_view);
            Console.SetCursorPosition(0, currentLineCursor);

            GC.Collect();
        }
    }
}
