﻿using ControllerLib;
using ControllerLib.XInput;
using System;

namespace ControllerCli
{
    public class Gamepad
    {
        public Gamepad()
        {

        }

        public Int32 RefreshRate { get; set; }
        public Boolean IsConnected { get; set; }
        public Boolean InputIsEnabled { get; set; }
        public GamepadInputState InputState { get; set; }
        public GamepadBatteryState BatteryState { get; set; }

        public String Buffer { get; private set; }

        public void Refresh(Synchronizer synchronizer, ControllerLib.XInput.Gamepad gamepad)
        {
            RefreshRate = synchronizer.RefreshRate;
            IsConnected = gamepad.IsConnected;
            InputIsEnabled = gamepad.InputIsEnabled;
            InputState = gamepad.InputState;
            BatteryState = gamepad.BatteryState;

            RefreshBuffer();
            Render();
        }
        public void Clear()
        {
            ClearBuffer();
            Render();
        }

        public void RefreshBuffer()
        {
            if (IsConnected)
            {             
                var BATTERY = String.Empty; var BATTERY_______ = String.Empty;
                switch (BatteryState.Level)
                {
                    case GamepadBatteryState.BatteryLevel.Empty:  BATTERY = "000%"; BATTERY_______ = " 000%           "; break;
                    case GamepadBatteryState.BatteryLevel.Low:    BATTERY = "033%"; BATTERY_______ = " ■■ 033%        "; break;
                    case GamepadBatteryState.BatteryLevel.Medium: BATTERY = "066%"; BATTERY_______ = " ■■■■■ 066%     "; break;
                    case GamepadBatteryState.BatteryLevel.Full:   BATTERY = "100%"; BATTERY_______ = " ■■■■■■■■■ 100% "; break;
                }

                var FPS = RefreshRate.ToString("D3");
                var INPUT_LOCK = InputIsEnabled ? "OFF" : "ON ";

                var K = InputState.BackPressed  ? "111" : "000";
                var S = InputState.StartPressed ? "111" : "000";

                var Q = InputState.LeftAnalogPressed ? "111" : "   ";
                var I = InputState.LeftAnalogX < ControllerLib.XInput.Gamepad.AnalogMinDeadzone ? "111" : "000";
                var O = InputState.LeftAnalogX > ControllerLib.XInput.Gamepad.AnalogMaxDeadzone ? "111" : "000";
                var P = InputState.LeftAnalogY < ControllerLib.XInput.Gamepad.AnalogMinDeadzone ? "111" : "000";
                var M = InputState.LeftAnalogY > ControllerLib.XInput.Gamepad.AnalogMaxDeadzone ? "111" : "000";

                var C = InputState.RightAnalogPressed ? "111" : "   ";
                var F = InputState.RightAnalogX < ControllerLib.XInput.Gamepad.AnalogMinDeadzone ? "111" : "000";
                var G = InputState.RightAnalogX > ControllerLib.XInput.Gamepad.AnalogMaxDeadzone ? "111" : "000";
                var H = InputState.RightAnalogY < ControllerLib.XInput.Gamepad.AnalogMinDeadzone ? "111" : "000";
                var T = InputState.RightAnalogY > ControllerLib.XInput.Gamepad.AnalogMaxDeadzone ? "111" : "000";

                var U = InputState.UpPressed    ? "111" : "000";
                var D = InputState.DownPressed  ? "111" : "000";
                var L = InputState.LeftPressed  ? "111" : "000";
                var R = InputState.RightPressed ? "111" : "000";

                var A = InputState.APressed ? "111" : "000";
                var B = InputState.BPressed ? "111" : "000";
                var X = InputState.XPressed ? "111" : "000";
                var Y = InputState.YPressed ? "111" : "000";

                var LB______ = InputState.LBPressed ? "1111111111" : "0000000000";
                var RB______ = InputState.RBPressed ? "1111111111" : "0000000000";

                var LT =   InputState.LTPressed > 0   ? "1111" : "0000";
                var LT_ =  InputState.LTPressed > 127 ? "11111" : "00000";
                var LT__ = InputState.LTPressed > 254 ? "111111" : "000000";
                var RT =   InputState.RTPressed > 0   ? "1111" : "0000";
                var RT_ =  InputState.RTPressed > 127 ? "11111" : "00000";
                var RT__ = InputState.RTPressed > 254 ? "111111" : "000000";

                Buffer = $@"Xbox Controller connected.                                 
                                                           
        {LT}                                    {RT}       
       {LT_}         +{BATTERY_______}+         {RT_}      
      {LT__}                                    {RT__}     
                                                           
      {LB______}    _______      _______    {RB______}     
   /  {LB______}  \         0000         /  {RB______}  \  
  /                \       000000       /                \ 
 /       {M}        \ _____ 0000 _____ /        {Y}       \
       00{M}00                                  {Y}        
     {I} {Q} {O}       {K}        {S}       {X}     {B}    
     {I} {Q} {O}       {K}        {S}       {X}     {B}    
       00{P}00                                  {A}        
         {P}     {U}                    {T}     {A}        
                 {U}                  00{T}00              
             {L}     {R}            {F} {C} {G}            
             {L}     {R}            {F} {C} {G}            
                 {D}                  00{H}00              
                 {D}                    {H}                
                                                           
                                                           
 ■■■■■■■ FPS: {FPS} | BATTERY: {BATTERY} | INPUT LOCK: {INPUT_LOCK} ■■■■■■■ 
";
            }
            else
            {
                Buffer = $@"Xbox Controller disconnected.                              
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
";
            }
        }
        public void ClearBuffer()
        {
            Buffer = $@"                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
                                                           
";
        }

        public void Render()
        {
            Int32 currentLineCursor = Console.CursorTop;
            Console.CursorVisible = false;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.WriteLine(Buffer);
            Console.SetCursorPosition(0, currentLineCursor);
        }
    }
}
