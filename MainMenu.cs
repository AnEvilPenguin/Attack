using Attack.Game;
using Attack.Options;
using Attack.Util;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MainMenu : Control
{
    private GameMaster _gameMaster;

    private VBoxContainer _container;

    private Button _back;

    private PanelContainer _loadPanel;
    private GridContainer _grid;

    private List<GameInstance> _games;

    private List<Node> _loadGameNodes = new List<Node>();

    public override void _Ready()
    {
        Log.Information("Loading main menu");
        _container = GetNode<VBoxContainer>("VBoxContainer");

        _gameMaster = GetNode<GameMaster>("/root/GameMaster");

        _back = GetNode<Button>("BackButton");

        _loadPanel = GetNode<PanelContainer>("PanelContainer");
        _grid = GetNode<GridContainer>("PanelContainer/GridContainer");


        if (_gameMaster.CanLoadGames())
        {
            Log.Debug("Can load games");

            var continueButton = GetNode<Button>("VBoxContainer/ContinueButton");
            var loadButton = GetNode<Button>("VBoxContainer/LoadGameButton");

            continueButton.Disabled = false;
            loadButton.Disabled = false;
        }

        _container.GetChildren()
            .OfType<Button>()
            .First(c => !c.Disabled)
            .GrabFocus();

        _games = _gameMaster.ListLoadableGames();

        OptionsManager.Load();
        Log.Debug("Main menu loaded");
    }

    public void OnContinuePressed()
    {
        Log.Debug("Continue");

        _gameMaster.Continue();
    }

    public void OnLoadGamePressed()
    {
        Log.Debug("LoadGame");

        LoadGameNodes();
        ToggleLoadVisibility();
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

    public void OnBackPressed()
    {
        Log.Debug("Back pressed");

        ToggleLoadVisibility();

        _loadGameNodes.ForEach(n => n.QueueFree());
    }

    private void ToggleLoadVisibility ()
    {
        _container.GetChildren()
            .OfType<Button>()
            .ToList()
            .ForEach(c => c.Visible = !c.Visible);

        _back.Visible = !_back.Visible;

        _loadPanel.Visible = !_loadPanel.Visible;
    }

    private void LoadGameNodes()
    {
        foreach (var game in _games)
        {
            var updated = new Label() { Text = game.UpdateDate?.ToString(Constants.DateStringFormat) };
            var button = new Button() { Text = "Load" };

            button.Pressed += () => _gameMaster.Load(game.Id);

            _loadGameNodes.Add(updated);
            _loadGameNodes.Add(button);
        }

        _loadGameNodes.ForEach(n => _grid.AddChild(n));
    }
}
