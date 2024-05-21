using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;

namespace Attack.Options
{
    internal static class OptionsManager
    {
        private static string _path = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "EvilPenguinIndustries\\Attack"
        );

        private const string _fileName = "Settings.json";

        private static Options _options;

        private static JsonSerializerOptions _serializerOptions = new JsonSerializerOptions {
            WriteIndented = true
        };

        static OptionsManager()
        {

            Directory.CreateDirectory(_path);

            Load();
        }

        public static List<Button> GetButtons() =>
            _options.GetButtons();

        public static void Save()
        {
            string jsonString = JsonSerializer.Serialize(_options, _serializerOptions);

            File.WriteAllTextAsync(Path.Combine(_path, _fileName), jsonString);
        }

        public static void Load()
        {
            string jsonString = String.Empty;

            var fullPath = Path.Combine(_path, _fileName);

            if (File.Exists(fullPath))
            {
                try
                {
                    jsonString = File.ReadAllText(fullPath);
                }
                catch (Exception ex)
                {
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
