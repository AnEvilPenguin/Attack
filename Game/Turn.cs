using Godot;
using Serilog;
using System;
using System.Collections.Generic;

namespace Attack.Game
{
    enum TurnAction
    {
        Invalid,
        Select,
        Move,
        Attack,
        EndGame
    }

    internal class Turn
    {
        public Tile SelectedTile { get; set; }
        public Tile DestinationTile { get; set; }
        public Tile AttackedTile { get; set; }

        Dictionary<Vector2I, Tile> ValidMoves = new Dictionary<Vector2I, Tile>();
        Dictionary<Vector2I, Tile> ValidAttacks = new Dictionary<Vector2I, Tile>();

        private PieceNode _selectedPiece;

        public Team TeamPlaying { get; set; }

        private string _turnSummary = String.Empty;

        internal Turn (Team currentTeam)
        {
            TeamPlaying = currentTeam;
        }

        public string TurnSummary 
        { 
            get
            { 
                if (_selectedPiece == null)
                    return _turnSummary;

                if (_selectedPiece.Spotted)
                    return $"{TeamPlaying} {_selectedPiece.PieceType} {_turnSummary}";

                return $"{TeamPlaying} piece {_turnSummary}";
            }
        }

        public TurnAction ProcessLeftClick(Tile tile, Vector2I cell)
        {
            if (SelectedTile == null)
            {
                if (tile.Piece == null)
                    return TurnAction.Invalid;

                if (tile.Piece.Team != TeamPlaying)
                    return TurnAction.Invalid;

                if (tile.Piece.Range < 1)
                    return TurnAction.Invalid;

                Log.Debug($"Tile selected {tile.Position}:{tile.Type}");

                _turnSummary = $"from {tile.Position}";

                SelectedTile = tile;
                _selectedPiece = tile.Piece;
                return TurnAction.Select;
            } 
            else if (SelectedTile != null && DestinationTile == null && tile.Piece?.Team == TeamPlaying)
            {
                // We've still got a valid selection here
                if (tile.Piece.Range < 1)
                    return TurnAction.Invalid;

                Log.Debug("Tile re-selected");

                _turnSummary = $"from {tile.Position}";

                SelectedTile = tile;
                _selectedPiece = tile.Piece;
                return TurnAction.Select;
            }


            if (DestinationTile == null && ValidMoves.ContainsKey(cell))
            {
                var currentPostion = SelectedTile.Position;

                var differenceX = Math.Max(currentPostion.X, cell.X) - Math.Min(currentPostion.X, cell.X);
                var differenceY = Math.Max(currentPostion.Y, cell.Y) - Math.Min(currentPostion.Y, cell.Y);

                // Can't move diagonally
                if (differenceX > 0 && differenceY > 0)
                    return TurnAction.Invalid;

                if (differenceX > SelectedTile.Piece.Range ||  differenceY > SelectedTile.Piece.Range)
                    return TurnAction.Invalid;

                DestinationTile = tile;

                DestinationTile.AddPiece(SelectedTile.Piece);
                SelectedTile.RemovePiece();

                _turnSummary += $" moves to {tile.Position}";

                return TurnAction.Move;
            }

            if (AttackedTile == null && ValidAttacks.ContainsKey(cell))
            {
                AttackedTile = tile;

                bool defenderIsFlag = tile.Piece.PieceType == PieceType.General;

                Tile aggressor = DestinationTile != null ?
                    DestinationTile :
                    SelectedTile;

                var result = aggressor.Piece.Attacks(tile.Piece);

                _turnSummary += $" and attacks {tile.Piece.Team} {tile.Piece.PieceType} at {tile.Position} resulting in a {result}";

                if (result == AttackResult.Victory)
                {
                    var piece = tile.Piece;
                    
                    tile.RemovePiece();
                    piece.QueueFree();
                }
                else if (result == AttackResult.Defeat)
                {
                    var piece = aggressor.Piece;

                    aggressor.RemovePiece();
                    piece.QueueFree();
                }

                if (defenderIsFlag)
                    return TurnAction.EndGame;

                return TurnAction.Attack;
            }

            if (DestinationTile != null)
                return TurnAction.Move;
            else if (SelectedTile != null)
                return TurnAction.Select;

            return TurnAction.Invalid;
        }

        public void ProcessRightClick()
        {
            // We can't allow attack actions to be reverted
            // would give the player free intel
            if (AttackedTile != null)
                return;

            if (SelectedTile == null)
                return;

            if (DestinationTile != null)
            {
                Log.Debug("Clearing move");

                SelectedTile.AddPiece(DestinationTile.Piece);
                DestinationTile.RemovePiece();
            }

            Log.Debug("Clearing selected tile");

            SelectedTile = null;
            DestinationTile = null;

            ValidMoves.Clear();
            ValidAttacks.Clear();
        }

        public void ClearMoves() =>
            ValidMoves.Clear();

        public void ClearAttacks() =>
            ValidAttacks.Clear();
        public void AddMove(Vector2I position, Tile tile)
        {
            if (ValidMoves.ContainsKey(position))
                return;

            ValidMoves.Add(position, tile);
        }
            

        public void AddAttack(Vector2I position, Tile tile)
        {
            if (ValidAttacks.ContainsKey(position))
                return;

            ValidAttacks.Add(position, tile);
        }
    }
}
