using Attack.Game;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoardMap : TileMap
{
	private enum MapLayer
	{
		Background,
		Pieces,
		Overlay,
		Highlight
	}

	[Export]
	public PackedScene PieceScene { get; set; }

	List<Tile> tiles = new List<Tile>();
	Dictionary<Vector2I, Tile> lookup = new Dictionary<Vector2I, Tile>();

	private Vector2 _offset;

	private GameMaster _gameMaster;

	internal void CreatePiece(Vector2I location, PieceType type, Team team)
	{
		Tile tile;

		Log.Debug($"Adding {type} to {location} for {team}");

		// TODO Some error handling?
        lookup.TryGetValue(location, out tile);

        PieceNode piece = PieceScene.Instantiate<PieceNode>();
        AddChild(piece);

		piece.Team = team;
		piece.PieceType = type;

        tile.AddPiece(piece);

		Log.Debug("Completed creating piece");
    }

	internal void AddPiece(PieceNode piece)
	{
		var location = (Vector2I)piece.Position;
        lookup.TryGetValue(location, out Tile tile);

		AddChild(piece);

		tile.AddPiece(piece);
    }

	internal List<Tile> ListPieces() =>
		tiles
			.Where(t => t.Piece != null)
			.ToList();
		

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Log.Debug("Readying board");

		_offset = GlobalPosition;
		int gridSize = Constants.GridSize;

		for (int i = 0; i < gridSize; i++)
		{
			for (int j = 0; j < gridSize; j++)
			{
				// TODO Extract
				TileType tileId = TileType.Terrain;

				if (i == 0 || j == 0 || i == gridSize - 1 || j == gridSize - 1)
					tileId = TileType.Border;

				bool startingTile = tileId == TileType.Terrain && j > gridSize - 6;

				var location = new Vector2I(i, j);

				SetCell((int)MapLayer.Background, location, (int)tileId, new Vector2I(0,0), 0);

				var tile = new Tile(location, MapToLocal(location), startingTile);
				tile.Type = tileId;

                tiles.Add(tile);
				lookup.Add(location, tile);
			}
		}

		Log.Debug("Completed readying board");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// TODO clean up this mess
	public override void _Process(double delta)
	{
		if (_gameMaster == null)
		{
            _gameMaster = GetNode<GameMaster>("/root/GameMaster");

            _gameMaster.CreateGame(this);
        }

		tiles.ForEach(t => EraseCell((int)MapLayer.Overlay, t.Position));

		var turn = _gameMaster.CurrentTurn;

		if (turn == null || turn.SelectedTile == null)
            tiles.ForEach(t => EraseCell((int)MapLayer.Highlight, t.Position));

        if (_gameMaster.NotificationShowing)
			return;

		// TODO if AI turn kick that process off and ignore player input

		var mousePosition = GetLocalMousePosition();
        var mapLocation = LocalToMap(mousePosition);

        if (!lookup.TryGetValue(mapLocation, out Tile tile))
        {
            Log.Debug("Click outside map");
            return;
        }

		if (tile.Type != TileType.Border)
			SetCell((int)MapLayer.Overlay, mapLocation, 0, new Vector2I(0,0), 0);

		if (Input.IsActionJustPressed("MouseClick"))
		{
            Log.Debug($"Clicked at {mapLocation}");

            if (_gameMaster.GameStarted)
			{
				ProcessGameLeftClick(tile);
				return;
			}

			ProcessSetupLeftClick(tile);
			return;
		}

		if (Input.IsActionJustPressed("RightClick"))
		{
            Log.Debug($"Right Clicked at {mapLocation}");

			if (_gameMaster.GameStarted)
            {
                ProcessGameRightClick();
                return;
            }

			ProcessSetupRightClick(tile);
			return;
        }
	}

	private void ProcessSetupLeftClick(Tile tile)
	{
		if (!tile.IsEmpty() || !_gameMaster.IsPiecePlaceable() || !tile.StartingTile)
			return;

        Log.Debug("Placeable tile");

        PieceNode piece = PieceScene.Instantiate<PieceNode>();
        AddChild(piece);

        piece.PieceType = _gameMaster.SelectedPieceType;
        piece.Team = Team.Blue;

        tile.AddPiece(piece);
        _gameMaster.AssignPiece();

        Log.Debug("Completed placement of tile");
    }

	private List<Tile> GetTilesAtRange(Tile tile, int range, bool includePieces)
	{
        var tiles = new List<Tile>();

        tiles = GetTilesAtRange(tile, range, TileSet.CellNeighbor.RightSide, tiles, includePieces);
        tiles = GetTilesAtRange(tile, range, TileSet.CellNeighbor.LeftSide, tiles, includePieces);
        tiles = GetTilesAtRange(tile, range, TileSet.CellNeighbor.TopSide, tiles, includePieces);
        tiles = GetTilesAtRange(tile, range, TileSet.CellNeighbor.BottomSide, tiles, includePieces);

		return tiles;
    }

	private List<Tile> GetTilesAtRange(Tile tile, int range, TileSet.CellNeighbor direction, List<Tile> tiles, bool includePieces)
	{
        bool canContinue = true;

        do
        {
            if (--range == 0)
                canContinue = false;

			// TODO consider extracting a lot of this
            var neighbor = GetNeighborCell(tile.Position, direction);

            if (lookup.TryGetValue(neighbor, out Tile neighbourTile))
            {
				if (!includePieces && neighbourTile.Type == TileType.Terrain)
				{
                    tiles.Add(neighbourTile);
                    tile = neighbourTile;
                }
                else if (includePieces && (neighbourTile.Type == TileType.Piece || neighbourTile.Type == TileType.Terrain))
                {
                    tiles.Add(neighbourTile);
                    tile = neighbourTile;
                }
                else
                {
                    canContinue = false;
                }
            }
            else
            {
                canContinue = false;
            }

        } while (canContinue);

        return tiles;
    }

    private void ProcessGameLeftClick(Tile tile)
    {
		Log.Debug("Left click in game");

        tiles.ForEach(t => EraseCell((int)MapLayer.Highlight, t.Position));

		var turn = _gameMaster.CurrentTurn;
		var turnAction = turn.ProcessLeftClick(tile, tile.Position);

		switch (turnAction)
		{
			case TurnAction.Select:
				Log.Debug("Piece selected");

				turn.ClearMoves();
                turn.ClearAttacks();

                var selectedTile = turn.SelectedTile;
				var moveTiles = GetTilesAtRange(selectedTile, selectedTile.Piece.Range, false);

				foreach (var neighbourtile in moveTiles)
				{
                    SetCell((int)MapLayer.Highlight, neighbourtile.Position, 3, new Vector2I(0, 0), 0);
					_gameMaster.CurrentTurn.AddMove(neighbourtile.Position, neighbourtile);
                }

                processAttackMarkers(selectedTile, turn);

                break;

			case TurnAction.Move:
				Log.Debug("Move action");

				turn.ClearMoves();
				turn.ClearAttacks();

				processAttackMarkers(turn.DestinationTile, turn);

				_gameMaster.CanCompleteTurn = true;

                break;

			case TurnAction.Attack:
				Log.Debug("End of turn");

				// No take backsies
				_gameMaster.CompleteTurn();
				return;

			case TurnAction.Invalid:
				Log.Debug("Invalid action");
				return;
		}
    }

	private void processAttackMarkers(Tile tile, Turn turn)
	{
		if (tile == null) 
			return;

        var attackTiles = GetTilesAtRange(tile, 1, true)
            .Where(t => t.Piece?.Team == Team.Red);

        foreach (var neighbourtile in attackTiles)
        {
            SetCell((int)MapLayer.Highlight, neighbourtile.Position, 4, new Vector2I(0, 0), 0);
            _gameMaster.CurrentTurn.AddAttack(neighbourtile.Position, neighbourtile);
        }
    }

	private void ProcessSetupRightClick(Tile tile)
	{
		if (tile.IsEmpty() && tile.Piece.Team != Team.Blue)
			return;

        var piece = tile.Piece;

        if (_gameMaster.IsPieceRemovable(piece.PieceType))
        {
            Log.Debug("Removable piece");

            tile.RemovePiece();
            _gameMaster.RemovePiece(piece.PieceType);

            piece.QueueFree();

            Log.Debug("Completed removal of tile");
        }
        else
        {
            Log.Error($"Error removing piece {piece.PieceType}");
        }
    }

	private void ProcessGameRightClick()
	{
		_gameMaster.CurrentTurn.ProcessRightClick();
        _gameMaster.CanCompleteTurn = false;
    }

	public void ClearAllOverlayElements()
	{
		foreach (var tile in tiles)
		{
            EraseCell((int)MapLayer.Highlight, tile.Position);
            EraseCell((int)MapLayer.Overlay, tile.Position);
        }
	}

	public void ReplayTurn(Vector2I[] turn)
	{
		// Convert into a turn and play it out.
		
		var start = turn[0];
		// There will always be a start

		Tile tile = lookup[start];
		ProcessGameLeftClick(tile); // probably a bodge
	
		var end = turn[1];

		// Zero is always off map so never valid
		if (end != Vector2I.Zero)
		{
            tile = lookup[end];
			ProcessGameLeftClick(tile);
        }

		var attack = turn[2];

        if (attack != Vector2I.Zero)
        {
            tile = lookup[attack];
            ProcessGameLeftClick(tile);
        }

		// End the turn
		_gameMaster.CompleteTurn(true);
    }
}