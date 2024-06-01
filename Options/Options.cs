using System;
using System.Collections.Generic;
using Attack.Util;
using Godot;
using Serilog;
using Serilog.Events;

namespace Attack.Options
{
    internal class Options
    {
        private bool _fullScreen = false;

        public bool FullScreen { 
            get => _fullScreen;
            set 
            {
                logChange("FullScreen", value.ToString());

                _fullScreen = value;

                var mode = _fullScreen ?
                    DisplayServer.WindowMode.Fullscreen :
                    DisplayServer.WindowMode.Windowed;

                DisplayServer.WindowSetMode(mode);
            }
        }

        private LogEventLevel _logLevel = LogEventLevel.Information;
        private string[] _logEventLevels = new string[]
        {
            "Error", "Warning", "Information", "Debug"
        };

        public LogEventLevel LogLevel
        {
            get => _logLevel;
            set
            {
                logChange("LogLevel", value.ToString());
                
                _logLevel = value;

                Logger.LevelSwitch.MinimumLevel = _logLevel;
            }
        }

        public Options() { }

        public List<Node> GetButtons()
        {
            Log.Debug("Getting option buttons");

            var buttons = new List<Node>()
            {
                new Label() { Text = "Fullscreen" },
                new OptionCheckButton(() => FullScreen = !FullScreen, _fullScreen),
                new Label() { Text = "Log Level" }
            };

            var logButton = new OptionOptionButton(_logEventLevels, _logLevel.ToString());
            logButton.ItemSelected += (long index) =>
            {
                var selectedItem = logButton.GetItemText((int)index);
                LogLevel = (LogEventLevel)Enum.Parse(typeof(LogEventLevel), selectedItem, true);
            };

            buttons.Add(logButton);

            Log.Debug("Completed getting option buttons");

            return buttons;
        }

        private void logChange(string variableName, string variableValue) =>
            Log.Debug($"Setting {variableName} to {variableValue}");
    }
}
