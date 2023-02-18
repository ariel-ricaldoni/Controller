using ControllerCli.View;
using ControllerLib;
using ControllerLib.Input;
using ControllerLib.Output;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ControllerCli
{
    public static class ConfigurationFactory
    {
        public const String ConfigurationFileName = "config.ini";
        public const String CommentSeparator = "#";
        public const String MacroSeparator = "+";

        public static Configuration GetConfiguration()
        {
            if (File.Exists(ConfigurationFileName))
            {
                var reader = new ConfigurationReader();
                return reader.Read();
            }
            else
            {
                var configuration = new Configuration();
                configuration.KeyBindings.Add(new KeyBinding());

                var writer = new ConfigurationWriter();
                writer.Write(configuration);

                return configuration;
            }
        }
    }

    public class ConfigurationReader
    {
        public Configuration Read()
        {
            var configuration = new Configuration();
            var keyBinding = new KeyBinding();

            Int32 lineNumber = 0;
            Int32 keyBindingCount = 0;

            String key = String.Empty;
            String value = String.Empty;

            FileReadLine(ConfigurationFactory.ConfigurationFileName, (line) =>
            {
                try
                {
                    lineNumber++;

                    key = String.Empty;
                    value = String.Empty;

                    RemoveWhiteSpace(ref line);

                    if (!IsValidLine(line)) return;

                    RemoveComment(ref line);

                    if (IsHeader(line))
                    {
                        AddKeyBindingToConfiguration(configuration, keyBinding, keyBindingCount);

                        keyBinding = new KeyBinding();

                        RemoveBrackets(ref line);

                        keyBindingCount++;
                    }

                    if (IsProperty(line))
                    {
                        GetKeyValue(line, out key, out value);

                        OverrideKey(ref key);

                        PropertyInfo propertyInfo;

                        if (TryGetPropertyInfo(configuration.GetType(), key, out propertyInfo))
                        {
                            SetPropertyValue(propertyInfo, configuration, value);
                        }

                        if (TryGetPropertyInfo(keyBinding.GetType(), key, out propertyInfo))
                        {
                            SetPropertyValue(propertyInfo, keyBinding, value);
                        }
                    }
                }
                catch
                {
                    if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(value))
                    {
                        throw new ArgumentException($"Invalid configuration file. Could not read line {line}.");
                    }
                    else
                    {
                        throw new ArgumentException($"Invalid configuration file. Could not read Value \"{value}\" on Parameter \"{key}\" on line {lineNumber}");
                    }
                }
            });

            AddKeyBindingToConfiguration(configuration, keyBinding, keyBindingCount);

            return configuration;
        }

        private void FileReadLine(string filePath, Action<string> lineReader)
        {
            using (var streamReader = File.OpenText(filePath))
            {
                String line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    lineReader(line);
                }
            }
        }
        private void RemoveWhiteSpace(ref String line)
        {
            line.Trim();
        }
        private Boolean IsValidLine(String line)
        {
            return !String.IsNullOrWhiteSpace(line) && line.IndexOf(ConfigurationFactory.CommentSeparator) != 0;
        }
        private void RemoveComment(ref String line)
        {
            var commentIndex = line.IndexOf(ConfigurationFactory.CommentSeparator);

            if (commentIndex > 0)
            {
                line.Substring(0, commentIndex);
            }
        }
        private Boolean IsHeader(String line)
        {
            return line.StartsWith("[") && line.EndsWith("]");
        }
        private Boolean IsProperty(String line)
        {
            return line.Contains("=");
        }
        private void RemoveBrackets(ref String line)
        {
            line = line.Replace("[", "").Replace("]", "");
        }
        private void GetKeyValue(String line, out String key, out String value)
        {
            var array = line.Split("=");

            key = array[0].Trim();
            value = array[1].Trim();
        }
        private void OverrideKey(ref String key)
        {
            if (key.ToLower().Equals("keybindingname"))
            {
                key = "Name";
            }
        }
        private Boolean TryGetPropertyInfo(Type type, String key, out PropertyInfo propertyInfo)
        {
            propertyInfo = type.GetProperty(key);

            return propertyInfo != null;
        }
        private void SetPropertyValue(PropertyInfo property, Object obj, String value)
        {
            if (String.IsNullOrWhiteSpace(value))
            {
                property.SetValue(obj, null); return;
            }

            switch (property.PropertyType.FullName)
            {
                case "System.String":
                    SetPropertyValue<String>(value, obj, property);
                    break;

                case "System.Int32":
                    SetPropertyValue<Int32>(value, obj, property);
                    break;

                case "System.Boolean":
                    SetPropertyValue<Boolean>(value, obj, property);
                    break;

                case "ControllerCli.View.ViewType":
                    SetPropertyValueEnum<ViewType>(value, obj, property);
                    break;

                case "ControllerLib.Input.IAnalogInput":
                    SetPropertyValueAnalogInput(value, obj, property);
                    break;

                case "ControllerLib.Input.IButtonInput":
                    SetPropertyValueButtonInput(value, obj, property);
                    break;
            }
        }
        private void SetPropertyValue<T>(String value, Object obj, PropertyInfo property)
        {
            var newValue = (T)Convert.ChangeType(value, typeof(T));

            property.SetValue(obj, newValue);
        }
        private void SetPropertyValueEnum<T>(String value, Object obj, PropertyInfo property) where T : Enum
        {
            var newValue = Helper.EnumParse<T>(value);

            property.SetValue(obj, newValue);
        }
        private void SetPropertyValueAnalogInput(String propertyValue, Object configuration, PropertyInfo configurationProperty)
        {
            if (propertyValue == "Cursor")
            {
                var cursor = new CursorInput();
                configurationProperty.SetValue(configuration, cursor);
            }
            else if (propertyValue == "Scroll")
            {
                var scroll = new ScrollInput();
                configurationProperty.SetValue(configuration, scroll);
            }
            else if (propertyValue == "WASD")
            {
                var wasdKeys = new DirectionalKeysInput(KeyboardKeyCode.W, KeyboardKeyCode.S, KeyboardKeyCode.A, KeyboardKeyCode.D);
                configurationProperty.SetValue(configuration, wasdKeys);
            }
            else if (propertyValue == "ArrowKeys")
            {
                var arrowKeys = new DirectionalKeysInput(KeyboardKeyCode.UpArrow, KeyboardKeyCode.DownArrow, KeyboardKeyCode.LeftArrow, KeyboardKeyCode.RightArrow);
                configurationProperty.SetValue(configuration, arrowKeys);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        private void SetPropertyValueButtonInput(String value, Object obj, PropertyInfo property)
        {
            Object output;

            if (value.Contains(ConfigurationFactory.MacroSeparator))
            {
                var valuesArr = value.Split(ConfigurationFactory.MacroSeparator);

                var values = new HashSet<KeyboardKeyCode>();
                foreach (var item in valuesArr)
                {
                    if (!Enum.TryParse(typeof(KeyboardKeyCode), item.Trim(), true, out output))
                    {
                        throw new ArgumentException();
                    }

                    values.Add((KeyboardKeyCode)output);
                }

                var keyboard = new KeyboardMacroInput(values);
                property.SetValue(obj, keyboard);
            }
            else if (Enum.TryParse(typeof(KeyboardKeyCode), value, true, out output))
            {
                var keyboard = new KeyboardKeyInput((KeyboardKeyCode)output);
                property.SetValue(obj, keyboard);
            }
            else if (Enum.TryParse(typeof(MouseKeyCode), value, true, out output))
            {
                var mouse = new MouseKeyInput((MouseKeyCode)output);
                property.SetValue(obj, mouse);
            }
            else
            {
                throw new ArgumentException();
            }
        }
        private void AddKeyBindingToConfiguration(Configuration configuration, KeyBinding keyBinding, Int32 keyBindingCount)
        {
            if (keyBindingCount > 0)
            {
                configuration.KeyBindings.Add(keyBinding);
            }
        }
    }

    public class ConfigurationWriter
    {
        public void Write(Configuration configuration)
        {
            var config = new StringBuilder();

            config.AppendLine($"# {Message.ControllerNameAndVersion}");
            config.AppendLine($"");
            config.AppendLine(@"# KEY BINDINGS MAP                                                                                   ");
            config.AppendLine(@"#                                                                                                    ");
            config.AppendLine(@"#                            0000                                    0000                            ");
            config.AppendLine(@"#                           00000                                    00000                           ");
            config.AppendLine(@"#           LTButton ----- 000000                     XButton        000000 ----- RTButton           ");
            config.AppendLine(@"#                                                           |                                        ");
            config.AppendLine(@"#                          0000000000          StartButton  |    0000000000                          ");
            config.AppendLine(@"#           LBButton ----- 0000000000                    |  |    0000000000 ----- RBButton           ");
            config.AppendLine(@"#                                           BackButton   |  |                                        ");
            config.AppendLine(@"#                             000           |            |  |        000 -------- YButton            ");
            config.AppendLine(@"#                           0000000         |            |  |        000                             ");
            config.AppendLine(@"#   LeftAnalogButton -------- 000 000       000        000  ---- 000     000                         ");
            config.AppendLine(@"#         LeftAnalog ---- 000 000 000       000        000       000     000 ----- BButton           ");
            config.AppendLine(@"#                           0000000                                  000                             ");
            config.AppendLine(@"#                             000     000 ----------         000     000 --------- AButton           ");
            config.AppendLine(@"#                                     000          |       0000000                                   ");
            config.AppendLine(@"#                                 000     000 ---  |     000 000 000 ---- RightAnalog                ");
            config.AppendLine(@"#         LeftButton ------------ 000     000   |  |     000 000 -------- RightAnalogButton          ");
            config.AppendLine(@"#                                     000       |  |       0000000                                   ");
            config.AppendLine(@"#         DownButton ---------------- 000       |  |         000                                     ");
            config.AppendLine(@"#                                               |  |                                                 ");
            config.AppendLine(@"#                                     RightButton  |                                                 ");
            config.AppendLine(@"#                                                  |                                                 ");
            config.AppendLine(@"#                                           UpButton                                                 ");
            config.AppendLine($"");
            config.AppendLine(@"# KEY BINDINGS OPTIONS                                                                                                                          ");
            config.AppendLine(@"# ■ *Analog                => Cursor, Scroll, ArrowKeys, WASD                                                                                   ");
            config.AppendLine(@"# ■ *Button (FOR MOUSE)    => LeftMouseButton, RightMouseButton, MiddleMouseButton                                                              ");
            config.AppendLine(@"# ■ *Button (FOR KEYBOARD) => Backspace, Tab, Clear, Enter, Shift, Ctrl, Alt, Pause, CapsLock, Esc, Space, PageUp, PageDown, End, Home,         ");
            config.AppendLine(@"#                             LeftArrow, UpArrow, RightArrow, DownArrow, Select, Print, Execute, PrintScreen, Ins, Del, Help, Num0, Num1, Num2, ");
            config.AppendLine(@"#                             Num3, Num4, Num5, Num6, Num7, Num8, Num9, A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, ");
            config.AppendLine(@"#                             Y, Z, Windows, LeftWindows, RightWindows, Application, Sleep, NumPad0, NumPad1, NumPad2, NumPad3, NumPad4,        ");
            config.AppendLine(@"#                             NumPad5, NumPad6, NumPad7, NumPad8, NumPad9, Multiply, Add, Separator, Subtract, Decimal, Divide, F1, F2, F3, F4, ");
            config.AppendLine(@"#                             F5, F6, F7, F8, F9, F10, F11, F12, NumLock, ScrollLock, LeftShift, RightShift, LeftCtrl, RightCtrl, LeftAlt,      ");
            config.AppendLine(@"#                             RightAlt, VolumeMute, VolumeDown, VolumeUp                                                                        ");
            config.AppendLine($"");
            config.AppendLine($"# IMPORTANT");
            config.AppendLine($"# ■ IF YOU WANT TO RESTORE CONFIGURATIONS TO DEFAULT, DELETE THIS FILE AND RUN THE ControlerCLI.exe, A NEW FILE WILL BE GENERATED AUTOMATICALLY.");
            config.AppendLine($"# ■ IF YOU WANT TO CREATE NEW KEY BINDINGS, COPY EVERYTHING FROM THE LINE STARTING AT \"[KeyBindingName=...]\" TILL THE END OF THE FILE, THEN");
            config.AppendLine($"#   CHANGE THE PROPERTIES ACCORDING TO THE ONES LISTED ABOVE IN THE \"KEYBINDINGS OPTIONS\" SECTION.");
            config.AppendLine($"# ■ IF IN THE COMMAND PROMPT PROPERTIES YOU HAVE THE \"Quick Edit Mode\" CHECKED, DON'T CLICK ON THE CONSOLE WINDOW, IT WILL CAUSE THE");
            config.AppendLine($"#   PROGRAM TO FREEZE. TO AVOID THIS, UNCHECK THE \"Quick Edit Mode\" PROPERTY.");
            config.AppendLine($"");
            config.AppendLine($"# OBSERVATIONS");
            config.AppendLine($"# ■ TO LOCK INPUT, PRESS AND HOLD \"START\" AND \"BACK\" BUTTONS TILL THE CONTROLLER STOPS TO VIBRATE. DO THE SAME TO UNLOCK INPUT.");
            config.AppendLine($"# ■ TO SWITCH KEY BINDINGS, LOCK THE INPUT THEN PRESS AND HOLD THE \"BACK\" BUTTON, USING THE \"LEFT\" AND \"RIGHT\" BUTTONS TO NAVIGATE.");
            config.AppendLine($"# ■ TO BE ABLE TO USE SOME WINDOWS FEATURES SUCH AS THE ON SCREEN KEYBOARD AND THE TASK MANAGER THE PROGRAM MUST BE EXECUTED AS ADMINISTRATOR.");
            config.AppendLine($"");
            config.AppendLine($"# KNOWN ISSUES");
            config.AppendLine($"# ■ XBOX SERIES GAMEPADS WON'T DISPLAY THE BATTERY PERCENTAGE ON WIRELESS MODE.");
            config.AppendLine($"");
            config.AppendLine($"# CONFIGURATION");
            config.AppendLine($"# Defines the amount of information that will be shown in the console window. Options are Complete, Basic and None.");
            config.AppendLine($"{nameof(Configuration.View)} = {configuration.View}");
            config.AppendLine($"");
            config.AppendLine($"# Defines if the console window should be resized according to the selected View. Should be set to \"false\" if you intend to run the application");
            config.AppendLine($"# from an already existing console window.");
            config.AppendLine($"{nameof(Configuration.ResizeWindow)} = {configuration.ResizeWindow}");
            config.AppendLine($"");

            foreach (var keyBinding in configuration.KeyBindings)
            {
                config.AppendLine($"[KeyBindingName={keyBinding.Name}]");
                config.AppendLine($"# Refresh rate per second the program will try to maintain, Value must be between {Synchronizer.TargetRefreshRateMinLimit} and {Synchronizer.TargetRefreshRateMaxLimit}. This is the number of \"Frames per Second\" aka FPS.");
                config.AppendLine($"# If you set this value to {Synchronizer.DefaultTargetRefreshRate} and the {nameof(KeyBinding.CursorMaxSpeed)} to {10} for example it means that in 1 second the cursor will move {(Synchronizer.DefaultTargetRefreshRate * 10)} pixels on your screen.");
                config.AppendLine($"# If you experience instability with frequent drop of refresh rate, lower this value until the refresh rate is stable or upgrade your system :P.");
                config.AppendLine($"{nameof(KeyBinding.TargetRefreshRate)} = {keyBinding.TargetRefreshRate}");
                config.AppendLine($"");
                config.AppendLine($"# Maximum speed of the mouse cursor in pixels per refresh, Value must be between {Mouse.MinCursorSpeedLimit} and {Mouse.MaxCursorSpeedLimit}.");
                config.AppendLine($"{nameof(KeyBinding.CursorMaxSpeed)} = {keyBinding.CursorMaxSpeed}");
                config.AppendLine($"");
                config.AppendLine($"# Multiplier speed of the scroll in relation to the mouse cursor, Value must be between {Mouse.MinScrollSpeedMultiplierLimit} and {Mouse.MaxScrollSpeedMultiplierLimit}. If the Value is 1 the scroll speed will be the");
                config.AppendLine($"# same as the mouse cursor.");
                config.AppendLine($"{nameof(KeyBinding.ScrollSpeedMultiplier)} = {keyBinding.ScrollSpeedMultiplier}");
                config.AppendLine($"");
                config.AppendLine($"# Delay before a Key held down starts to repeat its Value. Value must be between {Keyboard.MinKeyDownDelayLimit} and {Keyboard.MaxKeyDownDelayLimit}. If the Value equals TargetRefreshRate the delay will");
                config.AppendLine($"# be aproximately 1 second.");
                config.AppendLine($"{nameof(KeyBinding.KeyDownDelay)} = {keyBinding.KeyDownDelay}");
                config.AppendLine($"");
                config.AppendLine($"# Refer to the \"KEYBINDINGS OPTIONS\" section to change the following properties correctly.");
                config.AppendLine($"# Keyboard macros can be created by adding \"{ConfigurationFactory.MacroSeparator}\" between keyboard key binding options.");
                config.AppendLine($"# Example: \"Ctrl {ConfigurationFactory.MacroSeparator} Shift {ConfigurationFactory.MacroSeparator} Esc\" (opens the task manager).");
                config.AppendLine(WriteGamepadAnalogInputConfiguration(keyBinding.LeftAnalog, nameof(KeyBinding.LeftAnalog)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.LeftAnalogButton, nameof(KeyBinding.LeftAnalogButton)));
                config.AppendLine(WriteGamepadAnalogInputConfiguration(keyBinding.RightAnalog, nameof(KeyBinding.RightAnalog)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.RightAnalogButton, nameof(KeyBinding.RightAnalogButton)));
                config.AppendLine($"");
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.UpButton, nameof(KeyBinding.UpButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.DownButton, nameof(KeyBinding.DownButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.LeftButton, nameof(KeyBinding.LeftButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.RightButton, nameof(KeyBinding.RightButton)));
                config.AppendLine($"");
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.AButton, nameof(KeyBinding.AButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.BButton, nameof(KeyBinding.BButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.XButton, nameof(KeyBinding.XButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.YButton, nameof(KeyBinding.YButton)));
                config.AppendLine($"");
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.LBButton, nameof(KeyBinding.LBButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.LTButton, nameof(KeyBinding.LTButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.RBButton, nameof(KeyBinding.RBButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.RTButton, nameof(KeyBinding.RTButton)));
                config.AppendLine($"");
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.StartButton, nameof(KeyBinding.StartButton)));
                config.AppendLine(WriteGamepadButtonInputConfiguration(keyBinding.BackButton, nameof(KeyBinding.BackButton)));
                config.AppendLine($"# ----------------------------------------------------------------");
            }

            using (var writer = new StreamWriter(ConfigurationFactory.ConfigurationFileName, false))
            {
                writer.Write(config.ToString());
            }
        }

        private String WriteGamepadAnalogInputConfiguration(IAnalogInput input, String name)
        {
            if (input is CursorInput)
            {
                return $"{name} = Cursor";
            }
            else if (input is ScrollInput)
            {
                return $"{name} = Scroll";
            }
            else
            {
                return "";
            }
        }
        private String WriteGamepadButtonInputConfiguration(IButtonInput input, String name)
        {
            if (input is KeyboardMacroInput keyboardMacro)
            {
                var stringValue = new StringBuilder();
                foreach (var keyboardKeyInput in keyboardMacro.KeyboardKeyInputs)
                {
                    stringValue.Append((KeyboardKeyCode)keyboardKeyInput.KeyCode);
                    stringValue.Append($" {ConfigurationFactory.MacroSeparator} ");
                }
                stringValue.Remove(stringValue.Length - 3, 3);

                return $"{name} = {stringValue}";
            }
            else if (input is KeyboardKeyInput keyboard)
            {
                var enumValue = (KeyboardKeyCode)keyboard.KeyCode;
                String stringValue = enumValue.ToString() == "0" ? "" : enumValue.ToString();

                return $"{name} = {stringValue}";
            }
            else if (input is MouseKeyInput mouse)
            {
                var enumValue = (MouseKeyCode)mouse.KeyCode;
                String stringValue = enumValue.ToString() == "0" ? "" : enumValue.ToString();

                return $"{name} = {stringValue}";
            }
            else
            {
                return "";
            }
        }
    }
}
