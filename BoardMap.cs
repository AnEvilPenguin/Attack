using Attack.Game;
using Godot;
using System;
using System.Collections.Generic;

public partial class BoardMap : TileMap
{
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
				int tileId = 0;

				if (i == 0 || j == 0 || i == GridSize - 1 || j == GridSize - 1)
					tileId = 1;

				var location = new Vector2I(i, j);

				SetCell(0, location, tileId, new Vector2I(0,0), 0);

				var tile = new Tile(location, tileId);

                tiles.Add(tile);
				lookup.Add(location, tile);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		tiles.ForEach(t => EraseCell(1, t.Position));

		var mousePosition = GetLocalMousePosition();
        var mapLocation = LocalToMap(mousePosition);

		if (lookup.ContainsKey(mapLocation))
		{
			var tile = lookup[mapLocation];

			if (tile.Type != 1)
				SetCell(1, mapLocation, 2, new Vector2I(0,0), 0);
		}
	}
}
