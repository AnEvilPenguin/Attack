using Attack.Game;
using Godot;
using Serilog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.Time;

namespace Attack.Game
{
    internal class ArtificialPlayer
    {
        private BoardMap _board;
        private GameMaster _master;

        private Random _random = new Random();

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

            Log.Debug("Attempting to Attack Exposed");
            if (DirectAttackExposed())
                return;

            Log.Debug("Attempting to Attack Hidden");
            if (DirectAttackHidden())
                return;

            Log.Debug("Attempting to Move and then Attack Exposed");
            if (MoveThenAttackExposed())
                return;

            Log.Debug("Attempting to Move Scout");
            if (MoveScout())
                return;

            Log.Debug("Attempting to Move and then Attack Hidden");
            if (MoveThenAttackUnexposed())
                return;

            Log.Debug("Attempting to Move Engineer");
            if (MoveEngineer())
                return;

            Log.Debug("Attempting to Move Spy");
            if (MoveSpy())
                return;

            Log.Debug("Attempting to Move Low Ranks");
            if (MoveLowRank())
                return;

            Log.Debug("Attempting to Move High Ranks");
            if (MoveHighRank())
                return;

            // Oh dear
            MoveRandomly();
        }

        private bool MoveHighRank() =>
            MoveByRank(t => (int)t.Piece.PieceType > 5);

        private bool MoveLowRank() =>
            MoveByRank(t => (int)t.Piece.PieceType <= 5);

        private bool MoveByRank(Func<Tile, bool> filter)
        {
            var tiles = _friendlyTiles.Values
                .Where(filter)
                .Where(t => t.GetNeighbours()
                    .Any(n => _emptyTiles.ContainsKey(n)))
                .OrderBy(t => (int)t.Piece.PieceType);

            foreach (var tile in tiles)
            {
                int shortestDistance = -1;
                var closestEnemies = _enemyTiles.Values
                    .OrderBy(t =>
                    {
                        var abs = tile.DistanceFromTile(t).Abs();

                        int dist = abs.X + abs.Y;

                        if (shortestDistance == -1 || dist < shortestDistance)
                            shortestDistance = dist;

                        return dist;
                    })
                    .Where(t =>
                    {
                        var abs = tile.DistanceFromTile(t).Abs();
                        return abs.X + abs.Y == shortestDistance;
                    });

                // find empty tiles where distance is closer than shortest distance
                var closest = tile.GetNeighbours()
                    .Where(n => _emptyTiles.ContainsKey(n))
                    .Where(n => closestEnemies.Any(e =>
                    {
                        var abs = e.DistanceFromPosition(n).Abs();
                        return abs.X + abs.Y < shortestDistance;
                    }))
                    .FirstOrDefault();

                if (closest == Vector2I.Zero)
                    continue;

                var destination = _emptyTiles[closest];

                _board.PlayTurn(tile.Position, destination.Position, Vector2I.Zero);
                return true;
            }

            return false;
        }

        private bool MoveSpy()
        {
            if (!HasPieceType(_exposedEnemyTiles.Values, PieceType.Colonel))
                return false;

            if (!HasPieceType(_friendlyTiles.Values, PieceType.Spy))
                return false;

            var spy = _friendlyTiles.Values.First(t => t.Piece.PieceType == PieceType.Spy);
            var cic = _exposedEnemyTiles.Values.First(t => t.Piece.PieceType == PieceType.Colonel);

            var distance = spy.DistanceFromTile(cic);

            Tile destination = DirectionToDestination(spy, cic);

            if (destination == null)
                return false;

            _board.PlayTurn(spy.Position, destination.Position, Vector2I.Zero);
            return true;
        }

        private Tile DirectionToDestination(Tile source, Tile target)
        {
            var north = new Vector2I(0, -1);
            var south = new Vector2I(0, 1);
            var east = new Vector2I(1, 0);
            var west = new Vector2I(-1, 0);

            var distance = source.DistanceFromTile(target);

            Tile destination = null;

            if (distance.X > 0 && _emptyTiles.ContainsKey(source.Position + east))
                destination = _emptyTiles[source.Position + east];
            else if (distance.X < 0 && _emptyTiles.ContainsKey(source.Position + west))
                destination = _emptyTiles[source.Position + west];
            else if (distance.Y < 0 && _emptyTiles.ContainsKey(source.Position + north))
                destination = _emptyTiles[source.Position + north];
            else if (distance.Y > 0 && _emptyTiles.ContainsKey(source.Position + south))
                destination = _emptyTiles[source.Position + south];

            return destination;
        }

        private bool MoveEngineer()
        {
            if (!HasPieceType(_exposedEnemyTiles.Values, PieceType.Landmine))
                return false;

            if (!HasPieceType(_friendlyTiles.Values, PieceType.Engineer)) 
                return false;

            var north = new Vector2I(0, -1);
            var south = new Vector2I(0, 1);
            var east = new Vector2I(1, 0);
            var west = new Vector2I(-1, 0);

            var engineers = _friendlyTiles.Values.Where(t => t.Piece.PieceType == PieceType.Engineer);
            var landmines = _exposedEnemyTiles.Values.Where(t => t.Piece.PieceType == PieceType.Landmine);

            foreach (var engineer in engineers)
            {
                foreach (var mine in landmines)
                {
                    var distance = engineer.DistanceFromTile(mine);

                    var absolute = distance.Abs();

                    if (absolute.X + absolute.Y > 4)
                        continue;

                    Tile destination = DirectionToDestination(engineer, mine);

                    if (destination == null)
                        continue;

                    _board.PlayTurn(engineer.Position, destination.Position, Vector2I.Zero);
                    return true;
                }
            }

            return false;
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

        private List<Tuple<Tile, Tile>> getNeighboursAndTargets(Tile source, IEnumerable<Tile> validTargets)
        {
            var validNeigbours = source.GetNeighbours()
                .Where(n => _emptyTiles.ContainsKey(n));

            return validTargets
                .SelectMany(t => t.GetNeighbours(), (t, n) => new Tuple<Tile, Vector2I>(t, n))
                .Where(t => validNeigbours.Contains(t.Item2))
                .Select(t => new Tuple<Tile, Tile>(t.Item1, _emptyTiles[t.Item2]))
                .ToList();
        }

        private bool MoveThenAttackUnexposed()
        {
            var friendlyByRank = _friendlyTiles.Values
                .OrderBy(f => (int)f.Piece.PieceType)
                .ToList();

            foreach (var tile in friendlyByRank)
            {
                var targets = getNeighboursAndTargets(tile, _enemyTiles.Values);

                if (targets.Count > 0)
                {
                    var first = targets.First();

                    var destination = first.Item2;
                    var attack = first.Item1;

                    _board.PlayTurn(tile.Position, destination.Position, attack.Position);
                    return true;
                }
            }

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
                    .FirstOrDefault(n => neighbours.Contains(n) && _emptyTiles.ContainsKey(n));

                if (destination == Vector2I.Zero)
                    return false;

                _board.PlayTurn(attack.Item1.Position, destination, attack.Item2.Position);

                return true;
            }

            return false;
        }

        private void MoveRandomly()
        {
            // if we're here something has probably gone badly wrong.
            Log.Warning("Moving a random tile randomly");

            // Note that this is fine for small numbers, but there are much better alogrithms
            //     (that are more difficult to implement).
            // e.g. Fisher-Yates / Knuth
            var randomisedList = _friendlyTiles.Values.OrderBy(_ => _random.Next()).ToList();

            foreach (var tile in randomisedList)
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

            foreach (var selectedTile in friendlyNextToEnemy)
            {
                var target = selectedTile.GetNeighbours()
                    .Find(n => !_exposedEnemyTiles.ContainsKey(n) && _enemyTiles.ContainsKey(n));

                if (target == Vector2I.Zero)
                    continue;

                _board.PlayTurn(selectedTile.Position, Vector2I.Zero, target);
                return true;
            }

            return false;
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

        private bool HasPieceType(IEnumerable<Tile> tiles, PieceType type) =>
            tiles.Any(t => t.Piece != null && t.Piece.PieceType == type);
    }
}
