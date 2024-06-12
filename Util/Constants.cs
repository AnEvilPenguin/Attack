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
                { PieceType.Private, 4 },
                { PieceType.LanceCorporal, 4 },
                { PieceType.Corporal, 4 },
                { PieceType.Sergeant, 3 },
                { PieceType.Lieutenant, 2 },
                { PieceType.Captain, 1 },
                { PieceType.Colonel, 1 },
                { PieceType.General, 1 },
            };

        public const int GridSize = 12;
    }
}
