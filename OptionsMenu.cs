using Attack.Options;
using Godot;
using System;

public partial class OptionsMenu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		var optionsContainer = GetNode<VBoxContainer>("VBoxContainer");

        foreach (var button in OptionsManager.GetButtons())
        {
			optionsContainer.AddChild(button);
        }
    }

	public void OnBackButtonPressed()
	{
		GD.Print("Back");

		GetTree().ChangeSceneToFile("res://main_menu.tscn");
    }

	public void OnSaveButtonPressed()
		=> OptionsManager.Save();
}
