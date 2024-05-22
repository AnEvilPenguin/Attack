using System;
using System.Collections.Generic;
using Godot;
using Serilog;

namespace Attack.Options
{
    internal class Options
    {
        private bool _fullScreen = false;
        public bool FullScreen { 
            get => _fullScreen;
            set 
            {
                Log.Debug($"Setting FullScreen to {value}");

                _fullScreen = value;

                var mode = _fullScreen ?
                    DisplayServer.WindowMode.Fullscreen :
                    DisplayServer.WindowMode.Windowed;

                DisplayServer.WindowSetMode(mode);
            }
        }

        public Options() { }

        public List<Button> GetButtons()
        {
            var buttons = new List<Button>();

            buttons.Add(new OptionCheckButton("Fullscreen", () => FullScreen = !FullScreen, _fullScreen));

            return buttons;
        }

    }
}
