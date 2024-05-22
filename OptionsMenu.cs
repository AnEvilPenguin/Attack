using Attack.Options;
using Godot;
using Serilog;
using System;

public partial class OptionsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Log.Information("Options Menu");
		var optionsContainer = GetNode<GridContainer>("OptionsContainer");

        foreach (var button in OptionsManager.GetButtons())
        {
			optionsContainer.AddChild(button);
        }

		Log.Debug("Loaded Options Menu");
    }

	public void OnBackButtonPressed()
	{
		Log.Debug("Options Back");

		GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }

	public void OnSaveButtonPressed()
		=> OptionsManager.Save();
}
