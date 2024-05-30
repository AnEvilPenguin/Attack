using Attack.Game;
using Godot;
using System;

public partial class PieceSelector : Control
{
	private GameMaster _gameMaster;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        _gameMaster = GetNode<GameMaster>("/root/GameMaster");
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnButtonPressed(int id)
	{
		if (_gameMaster != null)
		{
			_gameMaster.SelectedPieceType = (PieceType)id;
        }
	}
}
