using System;
using System.IO;
using System.Collections.Generic;

namespace Attack.Util
{
    internal static class Constants
    {
        public static readonly string FolderPath = Path.Combine(
            System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData),
            "EvilPenguinIndustries\\Attack"
        );

        public const string DateStringFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'FFFFFFFK";

        static Constants()
        {
            Directory.CreateDirectory(FolderPath);
        }

        public static readonly Dictionary<PieceType, int> PieceLimits = new Dictionary<PieceType, int>
            {
                { PieceType.Landmine, 6 },
                { PieceType.Spy, 1 },
                { PieceType.Scout, 8 },
                { PieceType.Engineer, 5 },
                { PieceType.Sergeant, 4 },
                { PieceType.Lieutenant, 4 },
                { PieceType.Captain, 4 },
                { PieceType.Commandant, 3 },
                { PieceType.Colonel, 2 },
                { PieceType.BrigadierGeneral, 1 },
                { PieceType.CommanderInChief, 1 },
                { PieceType.Flag, 1 },
            };
    }
}
