using Attack.Game;
using Attack.Options;
using Attack.Util;
using Godot;
using Serilog;
using System;
using System.Linq;

public partial class MainMenu : Control
{
    private GameMaster _gameMaster;

    public override void _Ready()
    {
        Log.Information("Loading main menu");
        var container = GetNode<VBoxContainer>("VBoxContainer");

        _gameMaster = GetNode<GameMaster>("/root/GameMaster");


        if (_gameMaster.CanLoadGames())
        {
            Log.Debug("Can load games");

            var continueButton = GetNode<Button>("VBoxContainer/ContinueButton");
            var loadButton = GetNode<Button>("VBoxContainer/LoadGameButton");

            continueButton.Disabled = false;
            loadButton.Disabled = false;
        }

        container.GetChildren()
            .OfType<Button>()
            .First(c => !c.Disabled)
            .GrabFocus();

        OptionsManager.Load();
        Log.Debug("Main menu loaded");
    }

    public void OnContinuePressed()
    {
        Log.Debug("Continue");
    }

    public void OnLoadGamePressed()
    {
        Log.Debug("LoadGame");
    }

    public void OnNewGamePressed()
    {
        Log.Debug("NewGame");

        _gameMaster.New();
    }

    public void OnOptionsPressed()
    {
        Log.Debug("Options");

        GetTree().ChangeSceneToFile("res://options_menu.tscn");
    }
    public void OnQuitPressed()
    {
        Log.Information("Quitting via button");

        GetTree().Root.PropagateNotification((int)NotificationWMCloseRequest);
    }
}
