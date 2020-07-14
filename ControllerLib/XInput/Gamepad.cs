using System;

namespace ControllerLib.XInput
{
    public delegate void InputLockEventHandler();
    public delegate void AnalogEventHandler(Int32 stateX, Int32 stateY);
    public delegate void ButtonEventHandler(Boolean statePressed, Boolean previousStatePressed);

    public class Gamepad : Driver
    {    
        public Gamepad()
        {
            ReadState();
        }

        public const Int32 DefaultKeyDownDelay = 20;

        public const Int32 KeyDownDelayMinLimit = 6;
        public const Int32 KeyDownDelayMaxLimit = 120;

        public const Int16 AnalogMinValue = -32768;
        public const Int16 AnalogMaxValue = 32767;

        public const Int16 AnalogMinDeadzone = -6000;
        public const Int16 AnalogMaxDeadzone = 6000;

        public Boolean IsConnected { get { return InputState?.PacketNumber > 0; } }
        public Boolean InputIsEnabled { get; private set; } = true;       
        public Boolean StateChanged { get { return !BatteryState.Equals(PreviousBatteryState) || !InputState.Equals(PreviousInputState); } }     
        public GamepadInputState PreviousInputState { get; private set; }
        public GamepadInputState InputState { get; private set; }
        public GamepadBatteryState PreviousBatteryState { get; private set; }
        public GamepadBatteryState BatteryState { get; private set; }

        public Int32 KeyDownDelay
        {
            get { return _keyDownDelay; }
            set { _keyDownDelay = (value < KeyDownDelayMinLimit ? KeyDownDelayMinLimit : (value > KeyDownDelayMaxLimit ? KeyDownDelayMaxLimit : value)); }
        } private Int32 _keyDownDelay = DefaultKeyDownDelay;
        public Int32 KeyDown { get; private set; } = 0;
   
        public InputLockEventHandler InputLockHandler;

        public AnalogEventHandler LeftAnalogHandler;
        public ButtonEventHandler LeftAnalogButtonHandler;
        public AnalogEventHandler RightAnalogHandler;
        public ButtonEventHandler RightAnalogButtonHandler;

        public ButtonEventHandler UpButtonHandler;
        public ButtonEventHandler DownButtonHandler;
        public ButtonEventHandler LeftButtonHandler;
        public ButtonEventHandler RightButtonHandler;

        public ButtonEventHandler AButtonHandler;
        public ButtonEventHandler BButtonHandler;
        public ButtonEventHandler XButtonHandler;
        public ButtonEventHandler YButtonHandler;

        public ButtonEventHandler LBButtonHandler;
        public ButtonEventHandler LTButtonHandler;
        public ButtonEventHandler RBButtonHandler;
        public ButtonEventHandler RTButtonHandler;

        public ButtonEventHandler StartButtonHandler;
        public ButtonEventHandler BackButtonHandler;

        private readonly UInt32 _dwUserIndex = 0;
        private readonly byte _devType = new byte();

        private Boolean _inputIsEnabledTransition = true;

        public void HandleEvents()
        {
            InputLockHandler?.Invoke();

            if (InputIsEnabled == false) return;

            LeftAnalogHandler?.Invoke(InputState.LeftAnalogX, InputState.LeftAnalogY);
            LeftAnalogButtonHandler?.Invoke(InputState.LeftAnalogPressed, PreviousInputState.LeftAnalogPressed);
            RightAnalogHandler?.Invoke(InputState.RightAnalogX, InputState.RightAnalogY);
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
            XInputGetState(_dwUserIndex, ref inputState);

            PreviousInputState = InputState;
            InputState = new GamepadInputState(inputState);
            if (PreviousInputState == null) PreviousInputState = InputState;

            var batteryInformation = new XINPUT_BATTERY_INFORMATION();
            XInputGetBatteryInformation(_dwUserIndex, _devType, ref batteryInformation);

            PreviousBatteryState = BatteryState;
            BatteryState = new GamepadBatteryState(batteryInformation);
            if (PreviousBatteryState == null) PreviousBatteryState = BatteryState;
        }
        public void Vibrate(UInt16 motorSpeed)
        {
            Vibrate(_dwUserIndex, motorSpeed, motorSpeed);
        }

        public void AddKeyDown()
        {
            if (KeyDown < KeyDownDelay) KeyDown++;
        }
        public void ResetKeyDown()
        {
            KeyDown = 0;
        }

        public void ToggleInput()
        {
            InputIsEnabled = !_inputIsEnabledTransition;
        }
        public void DisableInput()
        {
            _inputIsEnabledTransition = InputIsEnabled;

            InputIsEnabled = false;
        }   
    }
}
