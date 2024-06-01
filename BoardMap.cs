using Attack.Game;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;

public partial class BoardMap : TileMap
{
	private enum MapLayer
	{
		Background,
		Pieces,
		Overlay
	}

	[Export]
	public PackedScene PieceScene { get; set; }

	int GridSize = 12;

	List<Tile> tiles = new List<Tile>();
	Dictionary<Vector2I, Tile> lookup = new Dictionary<Vector2I, Tile>();

	private Vector2 _offset;

	private GameMaster _gameMaster;

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

		var mousePosition = GetLocalMousePosition();
        var mapLocation = LocalToMap(mousePosition);

		if (lookup.ContainsKey(mapLocation))
		{
			var tile = lookup[mapLocation];

			if (tile.Type != TileType.Border)
				SetCell((int)MapLayer.Overlay, mapLocation, 0, new Vector2I(0,0), 0);
		}

		if (_gameMaster.GameStarted)
			return;

		if (Input.IsActionJustPressed("MouseClick"))
		{
			Log.Debug($"Clicked at {mapLocation}");

			if (!lookup.TryGetValue(mapLocation, out Tile tile))
				return;

			if (tile.IsEmpty() && _gameMaster.IsPiecePlaceable() && tile.StartingTile)
			{
				Log.Debug("Placeable tile");

                PieceNode piece = PieceScene.Instantiate<PieceNode>();
                AddChild(piece);

                piece.PieceType = _gameMaster.SelectedPieceType;
				piece.Team = Team.Blue;

                tile.AddPiece(piece);
				_gameMaster.AssignPiece();

                Log.Debug("Completed placement of tile");
            }
		}

		if (Input.IsActionJustPressed("RightClick"))
		{
            Log.Debug($"Right Clicked at {mapLocation}");

            if (!lookup.TryGetValue(mapLocation, out Tile tile))
                return;

            if (!tile.IsEmpty() && tile.Piece.Team == Team.Blue)
			{
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
        }
	}
}
