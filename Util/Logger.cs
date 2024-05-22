using Godot;
using Serilog;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Util
{
    internal partial class Logger : Node
    {
        private static bool configured = false;

        public static LoggingLevelSwitch LevelSwitch = new LoggingLevelSwitch();

        public override void _Ready()
        {
            if (!configured)
            {
                GetTree().AutoAcceptQuit = false;

                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.ControlledBy(LevelSwitch)
                    .WriteTo.Console()
                    .WriteTo.File(Path.Combine(Constants.FolderPath, "attack-.log"), rollingInterval: RollingInterval.Day)
                    .CreateLogger();

                Log.Information("The global logger has been configured");
                configured = true;
            }
        }

        public override void _Notification(int what)
        {
            if (what == NotificationWMCloseRequest)
            {
                Log.Information("Quit Notification received. Closing Logger.");

                Log.CloseAndFlush();
                GetTree().Quit();
            }
        }
    }
}
