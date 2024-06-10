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

            if (MoveThenAttackExposed())
                return;

            if (MoveScout())
                return;

            if (MoveThenAttackUnexposed())
                return;

            // TODO

            // If a piece with range is next to unexposed enemy
            // attack that piece
            // TODO consider if we want this to always happen, or priority (attack with lower ranks first?)

            // If Engineer within (4?) tiles of exposed Landmine move towards that

            // Pick random low ranked piece to move

            // Pick random high ranked piece to move

            // Oh dear
            MoveRandomly();
        }

        private bool MoveScout()
        {
            var scouts = _friendlyTiles
                .Values
                .Where(t => t.Piece.PieceType == PieceType.Scout)
                .ToList();

            int distance = 0;
            Tile selected = null;
            Tile destination = null;
            Tile attack = null;

            void rollup (List<Tile> tiles, Tile scout, TileSet.CellNeighbor dir)
            {
                if (tiles.Count <= distance)
                    return;

                var lastTile = tiles.Last();

                var enemy = lastTile.GetNeighbours()
                    .Where(n => !_exposedEnemyTiles.ContainsKey(n) && _enemyTiles.ContainsKey(n))
                    .FirstOrDefault();

                if (enemy == Vector2.Zero)
                    return;

                distance = tiles.Count;
                selected = scout;
                destination = lastTile;
                attack = _enemyTiles[enemy];
            };


            foreach (var scout in scouts)
            {
                var south = _board.GetTilesAtRange(scout, scout.Piece.Range, TileSet.CellNeighbor.BottomSide, new List<Tile>(), false);
                rollup(south, scout, TileSet.CellNeighbor.BottomSide);

                var east = _board.GetTilesAtRange(scout, scout.Piece.Range, TileSet.CellNeighbor.RightSide, new List<Tile>(), false);
                rollup(south, scout, TileSet.CellNeighbor.BottomSide);

                var west = _board.GetTilesAtRange(scout, scout.Piece.Range, TileSet.CellNeighbor.LeftSide, new List<Tile>(), false);
                rollup(south, scout, TileSet.CellNeighbor.BottomSide);

                var north = _board.GetTilesAtRange(scout, scout.Piece.Range, TileSet.CellNeighbor.TopSide, new List<Tile>(), false);
                rollup(south, scout, TileSet.CellNeighbor.BottomSide);
            }

            if (selected != null)
            {
                _board.PlayTurn(selected.Position, destination.Position, attack.Position);
                return true;
            }

            return false;
        }

        private bool MoveThenAttackUnexposed()
        {
            // TODO this
            return false;
        }

        private bool MoveThenAttackExposed()
        {
            // Find highest difference in value
            var attackableValues = new List<Tuple<Tile, Tile>>();

            foreach (var tile in _exposedFriendlyTiles.Values)
            {
                var attackableExposed = tile.GetLocationsWithinRange(true)
                        .Where(l => _exposedEnemyTiles.ContainsKey(l));

                foreach (var attackableTile in attackableExposed)
                {
                    attackableValues.Add(new Tuple<Tile, Tile>(tile, _exposedEnemyTiles[attackableTile]));
                }
            }

            if (attackableValues.Count > 0)
            {
                if(FindPieceToAttack(attackableValues))
                    return true;
            }

            foreach (var tile in _friendlyTiles.Values)
            {
                var attackableExposed = tile.GetLocationsWithinRange(true)
                        .Where(l => _exposedEnemyTiles.ContainsKey(l));

                foreach (var attackableTile in attackableExposed)
                {
                    attackableValues.Add(new Tuple<Tile, Tile>(tile, _exposedEnemyTiles[attackableTile]));
                }
            }

            return FindPieceToAttack(attackableValues);
        }

        private bool FindPieceToAttack(List<Tuple<Tile, Tile>> attackableValues)
        {
            if (attackableValues.Count < 0)
                return false;

            Tuple<Tile, Tile> attack = new Tuple<Tile, Tile>(null, null);
            int minPositive = 0;

            foreach (var tuple in attackableValues)
            {
                var value = (int)tuple.Item1.Piece.PieceType - (int)tuple.Item2.Piece.PieceType;
                if ((minPositive == 0 || value < minPositive) && value > 0)
                {
                    minPositive = value;
                    attack = tuple;
                }
            }

            if (minPositive > 0)
            {
                var neighbours = attack.Item2.GetNeighbours();
                var destination = attack.Item1
                    .GetNeighbours()
                    .First(n => neighbours.Contains(n));

                _board.PlayTurn(attack.Item1.Position, destination, attack.Item2.Position);

                return true;
            }

            return false;
        }

        private void MoveRandomly()
        {
            // if we're here something has probably gone badly wrong.
            Log.Warning("Moving a random tile randomly");

            foreach (var tile in _friendlyTiles.Values)
            {
                var destination = tile.GetNeighbours()
                    .FirstOrDefault(n => _emptyTiles.ContainsKey(n));

                if (destination == Vector2I.Zero)
                    continue;

                // doesn't matter if this is Zero or not
                var attack = _emptyTiles[destination].GetNeighbours()
                    .FirstOrDefault(n => _enemyTiles.ContainsKey(n));

                _board.PlayTurn(tile.Position, destination, attack);
                return;
            }
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
                .Find(n => !_exposedEnemyTiles.ContainsKey(n) && _enemyTiles.ContainsKey(n));

            _board.PlayTurn(selectedTile.Position, Vector2I.Zero, target);
            return true;

        }

        private void BuildLookups()
        {
            var tiles = _board.ListPieces();

            _emptyTiles = ConvertTileListToDictionary(_board.ListEmptyTiles());

            var friendlyTiles = tiles.Where(t => t.Piece.Team == Team.Red && t.Piece.Range > 0).ToList();
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
