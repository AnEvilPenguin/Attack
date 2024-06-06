using Attack.Game;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Game
{
    internal class ArtificialPlayer
    {
        private BoardMap _board;
        private GameMaster _master;

        private Dictionary<Vector2I, Tile> _friendlyTiles;
        private Dictionary<Vector2I, Tile> _exposedFriendlyTiles;
        private Dictionary<Vector2I, Tile> _enemyTiles;
        private Dictionary<Vector2I, Tile> _exposedEnemyTiles;
        private Dictionary<Vector2I, Tile> _emptyTiles;
        public ArtificialPlayer(BoardMap boardMap, GameMaster gameMaster)
        {
            _board = boardMap;
            _master = gameMaster;
        }

        public void ProcessTurn()
        {
            Log.Debug("Processing AI turn");

            BuildLookups();

            if (DirectAttackExposed())
                return;

            if (DirectAttackHidden())
                return;



            // TODO

            // If a piece with range is next to exposed enemy
            // and piece can capture enemy piece
            // capture/attack that piece

            // If a piece with range is next to unexposed enemy
            // attack that piece
            // TODO consider if we want this to always happen, or priority (attack with lower ranks first?)

            // If a scout can move more than 1 tile down the Y axis do that
            // attack if possible

            // If Engineer within (4?) tiles of exposed Landmine move towards that

            // Pick random low ranked piece to move

            // Pick random hight ranked piece to move
        }

        private bool DirectAttackExposed()
        {
            var friendlyNextToSpotted = GetTilesNextToExposed();

            if (friendlyNextToSpotted.Count != 0)
            {
                Log.Debug($"Found {friendlyNextToSpotted.Count} piece(s) next to an exposed enemy");

                foreach (var friendly in friendlyNextToSpotted)
                {
                    var selectedTile = friendlyNextToSpotted[0];
                    var selectedPiece = selectedTile.Piece;

                    // FIXME convert to list and process each exposed enemy for suitability
                    // OR order by rank and find first that guarantees victory
                    var target = selectedTile.GetNeighbours()
                        .Find(n => _exposedEnemyTiles.ContainsKey(n));

                    var targetPiece = _exposedEnemyTiles[target].Piece;

                    if (selectedPiece.Attacks(targetPiece) == AttackResult.Victory)
                    {
                        _board.PlayTurn(selectedTile.Position, Vector2I.Zero, target);
                        return true;
                    }

                    Log.Debug($"Not a sensible attack {selectedPiece.PieceType} => {targetPiece.PieceType}");
                }

                Log.Debug("Out of exposed pieces to attack");
            }

            return false;
        }

        private bool DirectAttackHidden()
        {
            var friendlyNextToEnemy = GetTilesNextToEnemies();

            if (friendlyNextToEnemy.Count == 0)
                return false;

            Log.Debug($"Found {friendlyNextToEnemy.Count} piece(s) next to an enemy");

            var selectedTile = friendlyNextToEnemy[0];

            var target = selectedTile.GetNeighbours()
                .Find(n => _enemyTiles.ContainsKey(n));

            _board.PlayTurn(selectedTile.Position, Vector2I.Zero, target);
            return true;

        }

        private void BuildLookups()
        {
            var tiles = _board.ListPieces();

            _emptyTiles = ConvertTileListToDictionary(_board.ListEmptyTiles());

            var friendlyTiles = tiles.Where(t => t.Piece.Team == Team.Red).ToList();
            _friendlyTiles = ConvertTileListToDictionary(friendlyTiles);

            var exposedFriendlyTiles = friendlyTiles.Where(t => t.Piece.Spotted).ToList();
            _exposedFriendlyTiles = ConvertTileListToDictionary(exposedFriendlyTiles);

            var enemyTiles = tiles.Where(t => t.Piece.Team == Team.Blue).ToList();
            _enemyTiles = ConvertTileListToDictionary(enemyTiles);

            var exposedEnemyTiles = enemyTiles.Where(t => t.Piece.Spotted).ToList();
            _exposedEnemyTiles = ConvertTileListToDictionary(exposedEnemyTiles);
        }

        private List<Tile> GetTilesNextToEnemies() =>
            GetTilesNextToPredicate(n => _enemyTiles.ContainsKey(n));

        private List<Tile> GetTilesNextToExposed() =>
            GetTilesNextToPredicate(n => _exposedEnemyTiles.ContainsKey(n));

        private List<Tile> GetTilesNextToPredicate(Predicate<Vector2I> action)
        {
            var tiles = new List<Tile>();

            foreach (var pair in _friendlyTiles)
            {
                var tile = pair.Value;

                bool nextToEnemy = tile.GetNeighbours()
                    .Find(action) != Vector2I.Zero;

                if (nextToEnemy)
                    tiles.Add(tile);
            }

            return tiles
                .OrderBy(t => (int)t.Piece.PieceType)
                .ToList();
        }

        private Dictionary<Vector2I, Tile> ConvertTileListToDictionary(List<Tile> tiles)
        {
            var dict = new Dictionary<Vector2I, Tile>();

            foreach (var tile in tiles)
            {
                if (dict.ContainsKey(tile.Position))
                    continue;

                dict.Add(tile.Position, tile);
            }

            return dict;
        }
    }
}
