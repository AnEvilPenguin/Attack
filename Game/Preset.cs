using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Game
{
    internal class Preset
    {
        public string Name;

        public string Description;

        public List<Tuple<Vector2I, PieceType>> Locations;
    }

    internal static class Presets
    {
        public static Preset StandardSetup = new Preset()
        {
            Name = "Standard",
            Description = "A basic example setup",
            Locations = new List<Tuple<Vector2I, PieceType>>
            {
                new Tuple<Vector2I, PieceType>(new Vector2I(1,1), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,1), PieceType.Flag),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,1), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,1), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,1), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,1), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,1), PieceType.Engineer),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,2), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,2), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,2), PieceType.Commandant),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,2), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,2), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,2), PieceType.Commandant),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,2), PieceType.Colonel),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,2), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,2), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,2), PieceType.Liutenant),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,3), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,3), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,3), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,3), PieceType.Spy),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,3), PieceType.Commandant),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,3), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,3), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,3), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,3), PieceType.Colonel),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,3), PieceType.Scout),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,4), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,4), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,4), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,4), PieceType.BrigadierGeneral),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,4), PieceType.CommanderInChief),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,4), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,4), PieceType.Landmine),
            }
        };
    }
}
