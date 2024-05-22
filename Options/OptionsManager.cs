using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Attack.Util;
using Godot;
using Serilog;

namespace Attack.Options
{
    internal static class OptionsManager
    {
        private const string _fileName = "Settings.json";

        private static Options _options;

        private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions {
            WriteIndented = true
        };

        static OptionsManager()
        {
            Load();
        }

        public static List<Button> GetButtons() =>
            _options.GetButtons();

        public static void Save()
        {
            Log.Information("Saving Settings");
            string jsonString = JsonSerializer.Serialize(_options, _serializerOptions);

            File.WriteAllTextAsync(Path.Combine(Constants.FolderPath, _fileName), jsonString);
        }

        public static void Load()
        {
            Log.Debug("Loading Settings");

            string jsonString = String.Empty;

            var fullPath = Path.Combine(Constants.FolderPath, _fileName);

            if (File.Exists(fullPath))
            {
                Log.Debug($"Loading settings from {fullPath}");

                try
                {
                    jsonString = File.ReadAllText(fullPath);
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                    GD.PrintErr(ex.ToString());
                }
            }

            if (jsonString != String.Empty)
            {
                _options = JsonSerializer.Deserialize<Options>(jsonString);
                return;
            }

            _options = new Options();
        }
    }
}
