using Attack.Game;
using Godot;
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

	int GridSize = 12;

	List<Tile> tiles = new List<Tile>();
	Dictionary<Vector2I, Tile> lookup = new Dictionary<Vector2I, Tile>();

	private Vector2 _offset;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_offset = GlobalPosition;

		for (int i = 0; i < GridSize; i++)
		{
			for (int j = 0; j < GridSize; j++)
			{
				TileType tileId = TileType.Terrain;

				if (i == 0 || j == 0 || i == GridSize - 1 || j == GridSize - 1)
					tileId = TileType.Border;

				var location = new Vector2I(i, j);

				SetCell((int)MapLayer.Background, location, (int)tileId, new Vector2I(0,0), 0);

				var tile = new Tile(location);
				tile.Type = tileId;

                tiles.Add(tile);
				lookup.Add(location, tile);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
	}
}
