using Godot;
using System;

public partial class Options : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Instantiate Options Class
		// Pass in VBOxContainer
		// Add controls based on Settings
	}

	public void OnBackPressed()
	{
		GD.Print("Back");
		GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }
}
