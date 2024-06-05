using Attack.Util;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Game
{
    internal enum TileType
    {
        Piece,
        Terrain,
        Border,
        Obstacle
    }

    internal class Tile
    {
        public Vector2I Position;

        public TileType? Type;

        public PieceNode Piece;

        public Vector2 LocalPosition;

        public bool StartingTile;

        public Tile (Vector2I position, Vector2 localPosition, bool startingTile)
        {
            Log.Debug($"Creating new {(startingTile ? "starting " : "")}tile at {position}:{localPosition}");

            Position = position;
            LocalPosition = localPosition;
            StartingTile = startingTile;
        }

        public void AddPiece(PieceNode piece)
        {
            Log.Debug($"Adding Piece {piece.PieceType} to {LocalPosition} for {piece.Team}");

            piece.Position = LocalPosition;
            Type = TileType.Piece;
            Piece = piece;
        }

        public PieceNode GetPiece => Piece;

        public void RemovePiece()
        {
            if (Piece == null)
                return;

            Log.Debug($"Removing Piece {Piece.PieceType} for {Piece.Team} from {LocalPosition}");

            Piece = null;
            Type = TileType.Terrain;
        }

        public bool IsEmpty() => Piece == null & Type == TileType.Terrain;

        public List<Vector2I> GetNeighbours()
        {
            var list = new List<Vector2I>();

            Vector2I north = Position - new Vector2I(0, 1);
            Vector2I east = Position - new Vector2I(1, 0);
            Vector2I south = Position + new Vector2I(0, 1);
            Vector2I west = Position + new Vector2I(1, 0);

            if(IsValidLocation(north))
                list.Add(north);
            if(IsValidLocation(east))
                list.Add(east);
            if (IsValidLocation(south))
                list.Add(south);
            if (IsValidLocation(west))
                list.Add(west);

            return list;
        }

        public List<Vector2I> GetOtherRowAndColumnLocations()
        {
            var list = new List<Vector2I>();

            for (int i = 1; i < Constants.GridSize -1; i++)
            {
                var x = new Vector2I(i, Position.Y);
                var y = new Vector2I(Position.X, i);

                if (x != Position)
                    list.Add(x);
                if (y != Position)
                    list.Add(y);
            }

            return list;
        }

        private bool IsValidLocation(Vector2I location) =>
            location.X > 0 &&
            location.Y > 0 &&
            location.X < Constants.GridSize - 1 &&
            location.Y < Constants.GridSize - 1;

    }
}
