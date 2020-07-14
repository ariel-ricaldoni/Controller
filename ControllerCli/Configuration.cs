using ControllerLib;
using ControllerLib.Configurations;
using ControllerLib.User32;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace ControllerCli
{
    public static class ConfigurationFactory
    {
        public const String ConfigurationFileName = "config.ini";

        public static Configuration GetConfiguration()
        {
            if (File.Exists(ConfigurationFileName))
            {
                return ReadConfigurationFile();
            }
            else
            {
                var configuration = new Configuration();

                WriteConfigurationFile(configuration);

                return configuration;
            }
        }

        private static Configuration ReadConfigurationFile()
        {
            var configuration = new Configuration();
            var configurationType = configuration.GetType();

            using (var fileStream = File.OpenRead(ConfigurationFileName))
            {
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8, true, 128))
                {
                    Int32 line = 0; String property;
                    while ((property = streamReader.ReadLine()) != null)
                    {
                        line++;

                        if (property.Trim() == String.Empty)
                        {
                            continue;
                        }

                        var comment = property.IndexOf("#");
                        
                        if (comment == 0)
                        {
                            continue;
                        }
                        else if (comment > 0)
                        {
                            property = property.Substring(0, comment);
                        }

                        var propertyArr = property.Split("=");

                        if (propertyArr.Length != 2)
                        {
                            throw new Exception(Message.InvalidConfigurationFile(line));
                        }

                        var propertyKey = propertyArr[0].Trim();
                        var propertyValue = propertyArr[1].Trim(); 

                        var configurationProperty = configurationType.GetProperty(propertyKey);

                        if (configurationProperty == null)
                        {
                            throw new Exception(Message.InvalidConfigurationFile(line, propertyKey, propertyValue));
                        }

                        if (String.IsNullOrEmpty(propertyValue) || propertyValue == "0")
                        {
                            configurationProperty.SetValue(configuration, null); continue;
                        }

                        switch(configurationProperty.PropertyType.FullName)
                        {
                            case "System.Int32":
                                ReadNumericInputConfiguration(line, propertyKey, propertyValue, configuration, configurationProperty);
                                break;

                            case "ControllerLib.Configurations.GamepadAnalogInput":
                                ReadGamepadAnalogInputConfiguration(line, propertyKey, propertyValue, configuration, configurationProperty);
                                break;

                            case "ControllerLib.Configurations.GamepadButtonInput":
                                ReadGamepadButtonInputConfiguration(line, propertyKey, propertyValue, configuration, configurationProperty);
                                break;

                            default:
                                throw new Exception($"Invalid configuration file. Could not read propertyValue \"{propertyValue}\" on parameter \"{propertyKey}\" on line {line}");
                        }
                    }
                }
            }

            return configuration;
        }
        private static void ReadNumericInputConfiguration(Int32 line, String propertyKey, String propertyValue, Object configuration, PropertyInfo configurationProperty)
        {
            Int32 intVal;

            if (Int32.TryParse(propertyValue, out intVal))
                configurationProperty.SetValue(configuration, intVal);
            else
                throw new Exception(Message.InvalidConfigurationFile(line, propertyKey, propertyValue));
        }
        private static void ReadGamepadAnalogInputConfiguration(Int32 line, String propertyKey, String propertyValue, Object configuration, PropertyInfo configurationProperty)
        {
            if (propertyValue == "Cursor")
            {
                var cursor = new MouseCursorInput();
                configurationProperty.SetValue(configuration, cursor);
            }
            else if (propertyValue == "Scroll")
            {
                var scroll = new MouseScrollInput();
                configurationProperty.SetValue(configuration, scroll);
            }
            else
            {
                throw new Exception(Message.InvalidConfigurationFile(line, propertyKey, propertyValue));
            }
        }
        private static void ReadGamepadButtonInputConfiguration(Int32 line, String propertyKey, String propertyValue, Object configuration, PropertyInfo configurationProperty)
        {
            Object output;

            if (Enum.TryParse(typeof(Mouse.KeyCode), propertyValue, true, out output))
            {
                var mouse = new MouseClickInput((Mouse.KeyCode)output);
                configurationProperty.SetValue(configuration, mouse);
            }
            else if (Enum.TryParse(typeof(Mouse.Macro), propertyValue, true, out output))
            {
                var mouse = new MouseMacroInput((Mouse.Macro)output);
                configurationProperty.SetValue(configuration, mouse);
            }
            else if (Enum.TryParse(typeof(Keyboard.KeyCode), propertyValue, true, out output))
            {
                var keyboard = new KeyboardKeyInput((Keyboard.KeyCode)output);
                configurationProperty.SetValue(configuration, keyboard);
            }
            else if (Enum.TryParse(typeof(Keyboard.Macro), propertyValue, true, out output))
            {
                var keyboard = new KeyboardMacroInput((Keyboard.Macro)output);
                configurationProperty.SetValue(configuration, keyboard);
            }
            else
            {
                throw new Exception(Message.InvalidConfigurationFile(line, propertyKey, propertyValue));
            }
        }

        private static void WriteConfigurationFile(Configuration configuration)
        {
            var config = new StringBuilder();

            config.AppendLine($"# {Message.ControllerNameAndVersion}");
            config.AppendLine($"");
            config.AppendLine($"# OBSERVATIONS");
            config.AppendLine($"# - IF YOU WANT TO RESTORE CONFIGURATIONS TO DEFAULT, DELETE THIS FILE AND EXECUTE THE PROGRAM, A NEW ONE WILL BE GENERATED AUTOMATICALLY.");
            config.AppendLine($"# - TO LOCK INPUT PRESS AND HOLD \"START\" AND \"BACK\" BUTTONS TILL THE CONTROLLER STOPS TO VIBRATE. DO THE SAME TO UNLOCK INPUT.");
            config.AppendLine($"# - TO BE ABLE TO USE THE ON SCREEN KEYBOARD THE PROGRAM MUST BE EXECUTED AS ADMINISTRATOR.");
            config.AppendLine($"# - IF IN THE COMMAND PROMPT PROPERTIES YOU HAVE THE \"Quick Edit Mode\" CHECKED, DON'T CLICK ON THE CONSOLE WINDOW, IT WILL CAUSE THE");
            config.AppendLine($"#   PROGRAM TO FREEZE. TO AVOID THIS, UNCHECK THE \"Quick Edit Mode\" PROPERTY.");
            config.AppendLine($"");
            config.AppendLine($"# Minimum speed of the mouse cursor in pixels per refresh, Value must be between {Mouse.CursorSpeedMinLimit} and CursorMaxSpeed.");
            config.AppendLine($"CursorMinSpeed = {configuration.CursorMinSpeed}");
            config.AppendLine($"");
            config.AppendLine($"# Maximum speed of the mouse cursor in pixels per refresh, Value must be between CursorMinSpeed and {Mouse.CursorSpeedMaxLimit}.");
            config.AppendLine($"CursorMaxSpeed = {configuration.CursorMaxSpeed}");
            config.AppendLine($"");
            config.AppendLine($"# Multiplier of the speed of the scroll in relation to the mouse cursor, Value must be between {Mouse.ScrollSpeedMultiplierMinLimit} and {Mouse.ScrollSpeedMultiplierMaxLimit}. If the Value is 1 the scroll speed will be the same as the mouse cursor.");
            config.AppendLine($"ScrollSpeedMultiplier = {configuration.ScrollSpeedMultiplier}");
            config.AppendLine($"");
            config.AppendLine($"# Delay before a Key held down starts to repeat its Value. Value must be between {Keyboard.KeyDownDelayMinLimit} and {Keyboard.KeyDownDelayMaxLimit}. If the Value equals TargetRefreshRate the delay will be aproximately 1 second.");
            config.AppendLine($"KeyDownDelay = {configuration.KeyDownDelay}");
            config.AppendLine($"");
            config.AppendLine($"# Refresh rate per second the controller will try to maintain, Value must be between {Synchronizer.TargetRefreshRateMinLimit} and {Synchronizer.TargetRefreshRateMaxLimit}. This is the number of \"Frames per Second\" aka FPS.");
            config.AppendLine($"TargetRefreshRate = {configuration.TargetRefreshRate}");
            config.AppendLine($"");
            config.AppendLine(WriteGamepadAnalogInputConfiguration(configuration.LeftAnalog, nameof(Configuration.LeftAnalog)));
            config.AppendLine(WriteGamepadAnalogInputConfiguration(configuration.RightAnalog, nameof(Configuration.RightAnalog)));
            config.AppendLine($"");
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.LeftAnalogButton, nameof(Configuration.LeftAnalogButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.RightAnalogButton, nameof(Configuration.RightAnalogButton)));
            config.AppendLine($"");
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.UpButton, nameof(Configuration.UpButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.DownButton, nameof(Configuration.DownButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.LeftButton, nameof(Configuration.LeftButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.RightButton, nameof(Configuration.RightButton)));
            config.AppendLine($"");
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.AButton, nameof(Configuration.AButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.BButton, nameof(Configuration.BButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.XButton, nameof(Configuration.XButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.YButton, nameof(Configuration.YButton)));
            config.AppendLine($"");
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.LBButton, nameof(Configuration.LBButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.LTButton, nameof(Configuration.LTButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.RBButton, nameof(Configuration.RBButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.RTButton, nameof(Configuration.RTButton)));
            config.AppendLine($"");
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.StartButton, nameof(Configuration.StartButton)));
            config.AppendLine(WriteGamepadButtonInputConfiguration(configuration.BackButton, nameof(Configuration.BackButton)));
        
            using (var writer = new StreamWriter(ConfigurationFileName, false))
            {
                writer.Write(config.ToString());
            }
        }
        private static String WriteGamepadAnalogInputConfiguration(GamepadAnalogInput input, String name)
        {
            if (input is MouseCursorInput)
            {
                return $"{name} = Cursor";
            }
            else if (input is MouseScrollInput)
            {
                return $"{name} = Scroll";
            }
            else
            {
                return "";
            }
        }
        private static String WriteGamepadButtonInputConfiguration(GamepadButtonInput input, String name)
        {
            if (input is KeyboardKeyInput keyboard)
            {
                var enumValue = (Keyboard.KeyCode)keyboard.KeyCode;
                String stringValue = enumValue.ToString() == "0" ? "" : enumValue.ToString();

                return $"{name} = {stringValue}";
            }
            else if (input is KeyboardMacroInput keyboardMacro)
            {
                var enumValue = keyboardMacro.Macro;
                String stringValue = enumValue.ToString() == "0" ? "" : enumValue.ToString();

                return $"{name} = {stringValue}";
            }
            else if (input is MouseClickInput mouse)
            {
                var enumValue = (Mouse.KeyCode)mouse.KeyCodeDown;
                String stringValue = enumValue.ToString() == "0" ? "" : enumValue.ToString();

                return $"{name} = {stringValue}";
            }
            else if (input is MouseMacroInput mouseMacro)
            {
                var enumValue = mouseMacro.Macro;
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
