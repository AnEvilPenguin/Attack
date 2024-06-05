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
    }
}
