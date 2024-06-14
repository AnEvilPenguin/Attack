using Attack.Game;
using Godot;
using Serilog;
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

    private Button _finishTurn;

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

        _finishTurn = GetNode<Button>("VBoxContainer/FinishTurn");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

        if (_gameMaster.GameStarted)
        {
            if (_finishTurn.Disabled && _gameMaster.CanCompleteTurn)
            {
                Log.Debug("Finish turn available");
                _finishTurn.Disabled = false;
                _finishTurn.Visible = true;
            }
            else if (!_finishTurn.Disabled && !_gameMaster.CanCompleteTurn)
            {
                Log.Debug("Finish turn not available");
                _finishTurn.Disabled = true;
                _finishTurn.Visible = false;
            }

            return;
        }


        _landmineButton.Text = $"{PieceType.Landmine} ({_gameMaster.GetPieceCount(PieceType.Landmine)})";
        _spyButton.Text = $"{PieceType.Spy} ({_gameMaster.GetPieceCount(PieceType.Spy)})";
        _scoutButton.Text = $"{PieceType.Scout} ({_gameMaster.GetPieceCount(PieceType.Scout)})";
        _engineerButton.Text = $"{PieceType.Engineer} ({_gameMaster.GetPieceCount(PieceType.Engineer)})";
        _sergeantButton.Text = $"{PieceType.Private} ({_gameMaster.GetPieceCount(PieceType.Private)})";
        _lieutenantButton.Text = $"{PieceType.LanceCorporal} ({_gameMaster.GetPieceCount(PieceType.LanceCorporal)})";
        _captainButton.Text = $"{PieceType.Corporal} ({_gameMaster.GetPieceCount(PieceType.Corporal)})";
        _commandantButton.Text = $"{PieceType.Sergeant} ({_gameMaster.GetPieceCount(PieceType.Sergeant)})";
        _colonelButton.Text = $"{PieceType.Lieutenant} ({_gameMaster.GetPieceCount(PieceType.Lieutenant)})";
        _brigadierGeneralButton.Text = $"{PieceType.Captain} ({_gameMaster.GetPieceCount(PieceType.Captain)})";
        _commanderInChiefButton.Text = $"{PieceType.Colonel} ({_gameMaster.GetPieceCount(PieceType.Colonel)})";
        _flagButton.Text = $"{PieceType.General} ({_gameMaster.GetPieceCount(PieceType.General)})";

        if (_startButton.Disabled && _gameMaster.IsPlacementComplete())
        {
            Log.Debug("Placement complete");
            _startButton.Disabled = false;
        }
        else if (!_startButton.Disabled && !_gameMaster.IsPlacementComplete())
        {
            Log.Debug("Placement no longer complete");
            _startButton.Disabled = true;
        }

    }

    public void OnButtonPressed(int id)
    {
        Log.Debug($"Piece button {id} pressed");

        if (_gameMaster == null)
        {
            Log.Error("Piece button pressed and gameMaster is null");
        }

        _gameMaster.SelectedPieceType = (PieceType)id;
    }

    public void OnStartButtonPressed()
    {
        Log.Debug("Game start pressed");

        _gameMaster.StartGame();

        DisableSettingButtons();
    }

    public void DisableSettingButtons()
    {
        _startButton.Disabled = true;

        _landmineButton.Visible = false;
        _spyButton.Visible = false;
        _scoutButton.Visible = false;
        _engineerButton.Visible = false;
        _sergeantButton.Visible = false;
        _lieutenantButton.Visible = false;
        _captainButton.Visible = false;
        _commandantButton.Visible = false;
        _colonelButton.Visible = false;
        _brigadierGeneralButton.Visible = false;
        _commanderInChiefButton.Visible = false;
        _flagButton.Visible = false;

        _startButton.Visible = false;
    }

    public void OnFinishTurnPressed()
    {
        Log.Debug("Finish Turn pressed");
        _gameMaster.CompleteTurn();
    }
}
