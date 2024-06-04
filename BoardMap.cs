using Attack.Game;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

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

	int GridSize = 12;

	List<Tile> tiles = new List<Tile>();
	Dictionary<Vector2I, Tile> lookup = new Dictionary<Vector2I, Tile>();

	private Vector2 _offset;

	private GameMaster _gameMaster;

	private Tile _selectedTile;
	private Tile _destinationTile;

	internal void createPiece(Vector2I location, PieceType type, Team team)
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

	internal List<Tile> ListPieces() =>
		tiles
			.Where(t => t.Piece != null)
			.ToList();
		

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Log.Debug("Readying board");

		_offset = GlobalPosition;

		for (int i = 0; i < GridSize; i++)
		{
			for (int j = 0; j < GridSize; j++)
			{
				// TODO Extract
				TileType tileId = TileType.Terrain;

				if (i == 0 || j == 0 || i == GridSize - 1 || j == GridSize - 1)
					tileId = TileType.Border;

				bool startingTile = tileId == TileType.Terrain && j > GridSize - 6;

				var location = new Vector2I(i, j);

				SetCell((int)MapLayer.Background, location, (int)tileId, new Vector2I(0,0), 0);

				var tile = new Tile(location, MapToLocal(location), startingTile);
				tile.Type = tileId;

                tiles.Add(tile);
				lookup.Add(location, tile);
			}
		}

		Log.Debug("Completed readying board");

        _gameMaster = GetNode<GameMaster>("/root/GameMaster");

		_gameMaster.CreateGame(this);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	// TODO clean up this mess
	public override void _Process(double delta)
	{
		tiles.ForEach(t => EraseCell((int)MapLayer.Overlay, t.Position));

		if (_selectedTile == null)
            tiles.ForEach(t => EraseCell((int)MapLayer.Highlight, t.Position));

        if (_gameMaster.NotificationShowing)
			return;

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
                ProcessGameRightClick(tile);
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

	private void SelectNormalPiece(Tile tile)
	{
        var location = tile.Position;

        var neighbours = GetSurroundingCells(location);

        foreach (var neighbor in neighbours)
        {
            if (!lookup.TryGetValue(neighbor, out Tile neighbourTile))
                continue;

            if (neighbourTile.IsEmpty())
            {
                SetCell((int)MapLayer.Highlight, neighbor, 3, new Vector2I(0, 0), 0);
                _selectedTile = tile;
            }
        }
    }

	private void SelectRangedPiece(Tile tile)
	{
        var tiles = new List<Tile>();
        var piece = tile.Piece;

		tiles = GetTileRange(tile, piece.Range, TileSet.CellNeighbor.RightSide, tiles);
        tiles = GetTileRange(tile, piece.Range, TileSet.CellNeighbor.LeftSide, tiles);
        tiles = GetTileRange(tile, piece.Range, TileSet.CellNeighbor.TopSide, tiles);
        tiles = GetTileRange(tile, piece.Range, TileSet.CellNeighbor.BottomSide, tiles);

		foreach (var neighbor in tiles)
		{
            SetCell((int)MapLayer.Highlight, neighbor.Position, 3, new Vector2I(0, 0), 0);

			if (_selectedTile == null)
				_selectedTile = tile;
        }

    }

	private List<Tile> GetTileRange(Tile tile, int range, TileSet.CellNeighbor direction, List<Tile> tiles)
	{
		bool canContinue = true;

		do
		{
			if (--range == 0)
				canContinue = false;

			var neighbor = GetNeighborCell(tile.Position, direction);

            if (lookup.TryGetValue(neighbor, out Tile neighbourTile))
			{
				if (neighbourTile.IsEmpty())
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

		if (_selectedTile == null)
		{
			// TODO mark valid attack choices
            if (tile.IsEmpty())
                return;

            if (tile.Piece.Team != Team.Blue)
                return;

            var piece = tile.Piece;

            if (piece.Range < 1)
                return;

            if (piece.Range == 1)
            {
                SelectNormalPiece(tile);
                return;
            }

            SelectRangedPiece(tile);
			return;
        }

        
		if (
			_destinationTile == null &&
			tile.IsEmpty() && 
			(tile.Position.X == _selectedTile.Position.X || tile.Position.Y == _selectedTile.Position.Y)
		)
		{
            _destinationTile = tile;

            var piece = _selectedTile.Piece;

			_selectedTile.RemovePiece();
            _destinationTile.AddPiece(piece);

			// Notify end of turn available?
			return;
		}


        // TODO check if there is piece selected.
        // if not check if piece can move
        // select piece
        // mark valid moves
        // mark valid attack choices
        // check if destination tile empty
        // check if valid move
        // move piece
        // mark valid attack choices
        // check if valid attack destination
        // profit?
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

	private void ProcessGameRightClick(Tile tile)
	{
		Log.Debug("Right click in game");
		_selectedTile = null;
	}
}