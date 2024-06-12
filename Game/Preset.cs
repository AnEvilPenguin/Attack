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

    // FIXME move to JSON file or something?
    // Should implement random (semi-random?) placement?
    internal static class Presets
    {
        public static Preset StandardSetup = new Preset()
        {
            Name = "Standard",
            Description = "A basic example setup",
            // FIXME some sort of verification against piece limits?
            Locations = new List<Tuple<Vector2I, PieceType>>
            {
                new Tuple<Vector2I, PieceType>(new Vector2I(1,1), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,1), PieceType.General),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,1), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,1), PieceType.Corporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,1), PieceType.LanceCorporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,1), PieceType.LanceCorporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,1), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,1), PieceType.Engineer),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,2), PieceType.Private),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,2), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,2), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,2), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,2), PieceType.LanceCorporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,2), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,2), PieceType.Lieutenant),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,2), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,2), PieceType.Private),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,2), PieceType.LanceCorporal),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,3), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,3), PieceType.Private),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,3), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,3), PieceType.Spy),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,3), PieceType.Sergeant),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,3), PieceType.Corporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,3), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,3), PieceType.Corporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,3), PieceType.Lieutenant),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,3), PieceType.Scout),

                new Tuple<Vector2I, PieceType>(new Vector2I(1,4), PieceType.Private),
                new Tuple<Vector2I, PieceType>(new Vector2I(2,4), PieceType.Landmine),
                new Tuple<Vector2I, PieceType>(new Vector2I(3,4), PieceType.Corporal),
                new Tuple<Vector2I, PieceType>(new Vector2I(4,4), PieceType.Captain),
                new Tuple<Vector2I, PieceType>(new Vector2I(5,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(6,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(7,4), PieceType.Colonel),
                new Tuple<Vector2I, PieceType>(new Vector2I(8,4), PieceType.Engineer),
                new Tuple<Vector2I, PieceType>(new Vector2I(9,4), PieceType.Scout),
                new Tuple<Vector2I, PieceType>(new Vector2I(10,4), PieceType.Landmine),
            }
        };
    }
}
