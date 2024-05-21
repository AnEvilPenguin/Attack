using System;
using System.IO;

namespace Attack.Util
{
    internal static class Constants
    {
        public static readonly string FolderPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "EvilPenguinIndustries\\Attack"
        );

        static Constants()
        {
            Directory.CreateDirectory(FolderPath);
        }
    }
}
