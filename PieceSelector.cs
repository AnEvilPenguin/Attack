using Attack.Game;
using Godot;
using System;

public partial class PieceSelector : Control
{
    private GameMaster _gameMaster;

    private Button _landmineButton;
    private Button _spyButton;
    private Button _scoutButton;
    private Button _engineerButton;
    private Button _sergeantButton;
    private Button _lieutenantButton;
    private Button _captainButton;
    private Button _commandantButton;
    private Button _colonelButton;
    private Button _brigadierGeneralButton;
    private Button _commanderInChiefButton;
    private Button _flagButton;

    private Button _startButton;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _gameMaster = GetNode<GameMaster>("/root/GameMaster");

        _landmineButton = GetNode<Button>("VBoxContainer/Landmine");
        _spyButton = GetNode<Button>("VBoxContainer/Spy");
        _scoutButton = GetNode<Button>("VBoxContainer/Scout");
        _engineerButton = GetNode<Button>("VBoxContainer/Engineer");
        _sergeantButton = GetNode<Button>("VBoxContainer/Sergeant");
        _lieutenantButton = GetNode<Button>("VBoxContainer/Lieutenant");
        _captainButton = GetNode<Button>("VBoxContainer/Captain");
        _commandantButton = GetNode<Button>("VBoxContainer/Commandant");
        _colonelButton = GetNode<Button>("VBoxContainer/Colonel");
        _brigadierGeneralButton = GetNode<Button>("VBoxContainer/BrigadierGeneral");
        _commanderInChiefButton = GetNode<Button>("VBoxContainer/CommanderInChief");
        _flagButton = GetNode<Button>("VBoxContainer/Flag");

        _startButton = GetNode<Button>("VBoxContainer/StartGame");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if (_gameMaster.GameStarted)
            return;

        _landmineButton.Text = $"Landmine ({_gameMaster.GetPieceCount(PieceType.Landmine)})";
        _spyButton.Text = $"Spy ({_gameMaster.GetPieceCount(PieceType.Spy)})";
        _scoutButton.Text = $"Scout ({_gameMaster.GetPieceCount(PieceType.Scout)})";
        _engineerButton.Text = $"Engineer ({_gameMaster.GetPieceCount(PieceType.Engineer)})";
        _sergeantButton.Text = $"Sergeant ({_gameMaster.GetPieceCount(PieceType.Sergeant)})";
        _lieutenantButton.Text = $"Lieutenant ({_gameMaster.GetPieceCount(PieceType.Lieutenant)})";
        _captainButton.Text = $"Captain ({_gameMaster.GetPieceCount(PieceType.Captain)})";
        _commandantButton.Text = $"Commandant ({_gameMaster.GetPieceCount(PieceType.Commandant)})";
        _colonelButton.Text = $"Colonel ({_gameMaster.GetPieceCount(PieceType.Colonel)})";
        _brigadierGeneralButton.Text = $"Brigadier General ({_gameMaster.GetPieceCount(PieceType.BrigadierGeneral)})";
        _commanderInChiefButton.Text = $"Commander-in-chief ({_gameMaster.GetPieceCount(PieceType.CommanderInChief)})";
        _flagButton.Text = $"Flag ({_gameMaster.GetPieceCount(PieceType.Flag)})";

        if (_gameMaster.IsPlacementComplete())
        {
            _startButton.Disabled = false;
        }

    }

    public void OnButtonPressed(int id)
    {
        if (_gameMaster != null)
        {
            _gameMaster.SelectedPieceType = (PieceType)id;
        }
    }
}
