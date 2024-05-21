using Godot;
using System;
using System.Linq;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        var container = GetNode<VBoxContainer>("VBoxContainer");

        container.GetChildren()
            .OfType<Button>()
            .First(c => !c.Disabled)
            .GrabFocus();
    }

    public void OnContinuePressed()
    {
        GD.Print("Continue");
    }

    public void OnLoadGamePressed()
    {
        GD.Print("LoadGame");
    }

    public void OnNewGamePressed()
    {
        GD.Print("NewGame");
    }

    public void OnOptionsPressed()
    {
        GD.Print("Options");

        GetTree().ChangeSceneToFile("res://options.tscn");
    }
    public void OnQuitPressed()
    {
        GD.Print("Quit");
        GetTree().Quit();
    }
}
