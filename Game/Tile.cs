using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Game
{
    internal class Tile
    {
        public Vector2I Position;

        public int Type;

        public Tile (Vector2I position, int type)
        {
            Position = position;
            Type = type;
        }
    }
}
