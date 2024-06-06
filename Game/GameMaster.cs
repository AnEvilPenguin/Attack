using Attack.Saves.SQLLite;
using Attack.Util;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Attack.Game
{
    internal partial class GameMaster : Node
    {
        private bool _initialized = false;

        // private stack previous turns
        // private stack replayed

        private SaveManager _sqlSaveManager;

        private GameInstance _gameInstance;

        public bool GameStarted = false;
        public bool CanCompleteTurn = false;
        public bool NotificationShowing = false;
        public bool LoadGame = false;
        public bool AiTurn = false;

        private List<PieceNode> _initialPlacements;
        private Queue<Vector2I[]> _protoTurns;

        private int _latestGameId = -1;

        public Turn CurrentTurn;

        private BoardMap _board;
        private Notification _notification;

        private Dictionary<PieceType, int> _playerPieceCount;

        public PieceType SelectedPieceType;

        private ArtificialPlayer _aiPlayer;

        public override void _Ready()
        {
            if (_initialized) return;

            Log.Debug("Initializing Game Master");

            // Connect/Create Database

            _sqlSaveManager = new SaveManager();
            _sqlSaveManager.Initialize();

            _initialized = true;

            Log.Debug("Game Master Initialized");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            if (CurrentTurn == null)
                return;

            if (CurrentTurn.TeamPlaying == Team.Red && AiTurn == false)
            {
                if (_aiPlayer == null)
                    _aiPlayer = new ArtificialPlayer(_board, this);

                AiTurn = true;

                _aiPlayer.ProcessTurn();

                AiTurn = false;
            }
        }

        public bool CanLoadGames()
        {
            _latestGameId = _sqlSaveManager.GetLatestSaveId();

            return _latestGameId != -1;
        }

        public void Continue()
        {
            _latestGameId = _sqlSaveManager.GetLatestSaveId();

            if (_latestGameId == -1)
            {
                Log.Error("Games not loadable (how are we here?)");
                return;
            }

            Load(_latestGameId);
        }

        public void Load(int id)
        {
            _gameInstance = _sqlSaveManager.LoadGame(_latestGameId);

            _initialPlacements = _sqlSaveManager.LoadPieces(_gameInstance);
            _protoTurns = _sqlSaveManager.LoadTurns(_gameInstance);

            LoadGame = true;

            GetTree().ChangeSceneToFile("res://game.tscn");
        }

        public void New()
        {
            Log.Debug("Creating new game");

            try
            {
                _gameInstance = _sqlSaveManager.NewGame();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create new game");
            }

            var random = new Random();
            int flip = random.Next(0, 2);
            _gameInstance.StartingTeam = (Team)flip;

            _sqlSaveManager.SaveGame(_gameInstance);

            GeneratePlayerPieceCount();

            Log.Debug("Loading game board");

            _latestGameId = _gameInstance.Id;

            GetTree().ChangeSceneToFile("res://game.tscn");
        }

        public void CreateGame(BoardMap board)
        {
            _board = board;

            if (LoadGame)
            {
                Log.Debug("Loading game from save data");

                LoadGameDataToBoard();

                var pieceSelector = GetNode<PieceSelector>("/root/Game/PieceSelector");
                pieceSelector.DisableSettingButtons();

                CurrentTurn = new Turn(_gameInstance.StartingTeam);

                // Replay all previous turns

                while (_protoTurns.Count > 0)
                {
                    _board.ReplayTurn(_protoTurns.Dequeue());
                }

                GameStarted = true;
                return;
            }

            Log.Debug("Creating game from presets");

            LoadGamePresetToBoard();

            Log.Debug("Completed creating game");
        }

        private void LoadGamePresetToBoard()
        {
            foreach (var pieceConfig in Presets.StandardSetup.Locations)
            {
                var location = pieceConfig.Item1;
                var pieceType = pieceConfig.Item2;

                _board.CreatePiece(location, pieceType, Team.Red);
            }
        }

        private void LoadGameDataToBoard()
        {
            GeneratePlayerPieceCount();

            // This loop is the other half of a giant bodge.
            // We need to instantiate the piece from the board otherwise we lose things like shaders and materials.
            // Otherwise I need to create that from code from scratch
            //     (as I should do next time I intend to do this sort of thing)
            foreach (var piece in _initialPlacements) 
            {
                _board.CreatePiece((Vector2I)piece.Position, piece.PieceType, piece.Team);

                if(piece.Team == Team.Blue)
                    _playerPieceCount[piece.PieceType]++;

                piece.QueueFree();
            }
        }

        private void GeneratePlayerPieceCount() =>
            _playerPieceCount = new Dictionary<PieceType, int>
            {
                { PieceType.Landmine, 0 },
                { PieceType.Spy, 0 },
                { PieceType.Scout, 0 },
                { PieceType.Engineer, 0 },
                { PieceType.Sergeant, 0 },
                { PieceType.Lieutenant, 0 },
                { PieceType.Captain, 0 },
                { PieceType.Commandant, 0 },
                { PieceType.Colonel, 0 },
                { PieceType.BrigadierGeneral, 0 },
                { PieceType.CommanderInChief, 0 },
                { PieceType.Flag, 0 },
            };

        public void StartGame()
        {
            GameStarted = true;

            _gameInstance.StartingPositions = _board.ListPieces();
            _gameInstance.StartDate = DateTime.UtcNow;

            _sqlSaveManager.SaveGame(_gameInstance);

            _notification.SendNotification($"{_gameInstance.StartingTeam} to start!");
            NotificationShowing = true;

            CurrentTurn = new Turn(_gameInstance.StartingTeam);
        }

        public int GetPieceCount(PieceType pieceType) =>
            Constants.PieceLimits[pieceType] - _playerPieceCount[pieceType];

        public bool IsPiecePlaceable() =>
            _playerPieceCount[SelectedPieceType] < Constants.PieceLimits[SelectedPieceType];

        public bool IsPieceRemovable(PieceType type) =>
            _playerPieceCount[type] > 0;

        public void AssignPiece() =>
            _playerPieceCount[SelectedPieceType]++;

        public void RemovePiece(PieceType type) =>
            _playerPieceCount[type]--;

        public bool IsPlacementComplete() =>
            !_playerPieceCount
                .Keys
                .Any(k => _playerPieceCount[k] != Constants.PieceLimits[k]);

        public override void _Notification(int what)
        {
            if (what == NotificationWMCloseRequest)
            {
                if(GameStarted)
                    return;

                Log.Debug("Cleaning up unstarted game");

                _sqlSaveManager.DeleteGame(_gameInstance);
                _gameInstance = null;
            }
        }

        public void RegisterNotification(Notification notification) =>
            _notification = notification;

        public void CompleteTurn(bool replay = false)
        {
            // TODO entire forward/back feature.
            if (replay)
            {
                Log.Debug("End of replay turn");
            }
            else
            {
                Log.Information("End of turn");
            }

            _board.ClearAllOverlayElements();

            if (!replay)
                _sqlSaveManager.SaveGame(_gameInstance, CurrentTurn);

            // TODO push turn into stack here
            CurrentTurn = new Turn(CurrentTurn.TeamPlaying == Team.Red ? Team.Blue : Team.Red);

            CanCompleteTurn = false;
        }
    }
}
