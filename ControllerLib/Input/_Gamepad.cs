using ControllerLib.Driver;
using System;

namespace ControllerLib.Input
{
    public interface IAnalogInput { }
    public interface IButtonInput { }

    public delegate void InternalEventHandler();
    public delegate void AnalogEventHandler(Int32 stateX, Int32 stateY, Int32 previousStateX, Int32 previousStateY);
    public delegate void ButtonEventHandler(Boolean statePressed, Boolean previousStatePressed);

    public class Gamepad
    {
        public Gamepad()
        {
            _driver = new XInput();
        }
        public Gamepad(IXInput driver)
        {
            _driver = driver;
        }

        public const Int32 DefaultKeyDownDelay = 20;
        public const Int32 MinKeyDownDelayLimit = 6;
        public const Int32 MaxKeyDownDelayLimit = 144;
        public const Int16 MinAnalogLimit = -32768;
        public const Int16 MaxAnalogLimit = 32767;
        public const Int16 MinAnalogDeadzone = -6000;
        public const Int16 MaxAnalogDeadzone = 6000;

        public Boolean IsConnected { get { return InputState?.PacketNumber > 0; } }
        public Boolean IsEnabled { get; private set; } = true;
        public Boolean PreviousIsEnabled { get; private set; } = false;
        public Boolean StateChanged { get { return !BatteryState.Equals(PreviousBatteryState) || !InputState.Equals(PreviousInputState); } }
        
        public Int32 KeyDownDelay
        {
            get { return _keyDownDelay; }
            set { _keyDownDelay = (value < MinKeyDownDelayLimit ? MinKeyDownDelayLimit : (value > MaxKeyDownDelayLimit ? MaxKeyDownDelayLimit : value)); }
        }
        public Int32 KeyDown { get; private set; } = 0;

        public InputState InputState { get; private set; }
        public InputState PreviousInputState { get; private set; }
        public BatteryState BatteryState { get; private set; }
        public BatteryState PreviousBatteryState { get; private set; }

        public InternalEventHandler SystemHandler { get; set; }

        public AnalogEventHandler LeftAnalogHandler { get; set; }
        public ButtonEventHandler LeftAnalogButtonHandler { get; set; }
        public AnalogEventHandler RightAnalogHandler { get; set; }
        public ButtonEventHandler RightAnalogButtonHandler { get; set; }

        public ButtonEventHandler UpButtonHandler { get; set; }
        public ButtonEventHandler DownButtonHandler { get; set; }
        public ButtonEventHandler LeftButtonHandler { get; set; }
        public ButtonEventHandler RightButtonHandler { get; set; }

        public ButtonEventHandler AButtonHandler { get; set; }
        public ButtonEventHandler BButtonHandler { get; set; }
        public ButtonEventHandler XButtonHandler { get; set; }
        public ButtonEventHandler YButtonHandler { get; set; }

        public ButtonEventHandler LBButtonHandler { get; set; }
        public ButtonEventHandler LTButtonHandler { get; set; }
        public ButtonEventHandler RBButtonHandler { get; set; }
        public ButtonEventHandler RTButtonHandler { get; set; }

        public ButtonEventHandler StartButtonHandler { get; set; }
        public ButtonEventHandler BackButtonHandler { get; set; }

        private readonly UInt32 _dwUserIndex = 0;
        private readonly Byte _devType = new Byte();

        private Int32 _keyDownDelay = DefaultKeyDownDelay;

        private IXInput _driver { get; set; }

        public void HandleEvents()
        {
            SystemHandler?.Invoke();

            if (!IsEnabled) return;

            LeftAnalogHandler?.Invoke(InputState.LeftAnalogX, InputState.LeftAnalogY, PreviousInputState.LeftAnalogX, PreviousInputState.LeftAnalogY);
            LeftAnalogButtonHandler?.Invoke(InputState.LeftAnalogPressed, PreviousInputState.LeftAnalogPressed);
            RightAnalogHandler?.Invoke(InputState.RightAnalogX, InputState.RightAnalogY, PreviousInputState.RightAnalogX, PreviousInputState.RightAnalogY);
            RightAnalogButtonHandler?.Invoke(InputState.RightAnalogPressed, PreviousInputState.RightAnalogPressed);

            UpButtonHandler?.Invoke(InputState.UpPressed, PreviousInputState.UpPressed);
            DownButtonHandler?.Invoke(InputState.DownPressed, PreviousInputState.DownPressed);
            LeftButtonHandler?.Invoke(InputState.LeftPressed, PreviousInputState.LeftPressed);
            RightButtonHandler?.Invoke(InputState.RightPressed, PreviousInputState.RightPressed);

            AButtonHandler?.Invoke(InputState.APressed, PreviousInputState.APressed);
            BButtonHandler?.Invoke(InputState.BPressed, PreviousInputState.BPressed);
            XButtonHandler?.Invoke(InputState.XPressed, PreviousInputState.XPressed);
            YButtonHandler?.Invoke(InputState.YPressed, PreviousInputState.YPressed);

            LBButtonHandler?.Invoke(InputState.LBPressed, PreviousInputState.LBPressed);
            LTButtonHandler?.Invoke(InputState.LTPressed > 0, PreviousInputState.LTPressed > 0);
            RBButtonHandler?.Invoke(InputState.RBPressed, PreviousInputState.RBPressed);
            RTButtonHandler?.Invoke(InputState.RTPressed > 0, PreviousInputState.RTPressed > 0);

            StartButtonHandler?.Invoke(InputState.StartPressed, PreviousInputState.StartPressed);
            BackButtonHandler?.Invoke(InputState.BackPressed, PreviousInputState.BackPressed);
        }

        public void ReadState()
        {
            var inputState = new XINPUT_STATE();
            _driver.GetState(_dwUserIndex, ref inputState);

            PreviousInputState = InputState;
            InputState = new InputState(inputState);
            if (PreviousInputState == null) PreviousInputState = InputState;

            var batteryInformation = new XINPUT_BATTERY_INFORMATION();
            _driver.GetBatteryInformation(_dwUserIndex, _devType, ref batteryInformation);

            PreviousBatteryState = BatteryState;
            BatteryState = new BatteryState(batteryInformation);
            if (PreviousBatteryState == null) PreviousBatteryState = BatteryState;
        }
        public void Vibrate(UInt16 motorSpeed)
        {
            var inputVibration = new XINPUT_VIBRATION();
            inputVibration.wLeftMotorSpeed = motorSpeed;
            inputVibration.wRightMotorSpeed = motorSpeed;

            _driver.SetState(_dwUserIndex, ref inputVibration);
        }

        public void AddKeyDown()
        {
            if (KeyDown < KeyDownDelay) KeyDown++;
        }
        public void ResetKeyDown()
        {
            KeyDown = 0;
        }

        public void EnableInput()
        {
            PreviousIsEnabled = IsEnabled;
            IsEnabled = true;
        }
        public void DisableInput()
        {
            PreviousIsEnabled = IsEnabled;
            IsEnabled = false;
        }
    }
}
