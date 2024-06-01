using Attack.Saves.SQLLite;
using Attack.Util;
using Godot;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private BoardMap _board;

        private Dictionary<PieceType, int> _playerPieceCount;

        public PieceType SelectedPieceType;

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

            _sqlSaveManager.SaveGame(_gameInstance);

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


            Log.Debug("Loading game board");

            GetTree().ChangeSceneToFile("res://game.tscn");
        }

        public void CreateGame(BoardMap board)
        {
            Log.Debug("Creating game from presets");

            _board = board;

            foreach (var pieceConfig in Presets.StandardSetup.Locations)
            {
                var location = pieceConfig.Item1;
                var pieceType = pieceConfig.Item2;

                _board.createPiece(location, pieceType, Team.Red);
            }

            Log.Debug("Completed creating game");
        }

        public void StartGame()
        {
            GameStarted = true;
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

        // NewGame
        // create new game
        // create teams?
        // add computer pieces to board
        // add user pieces to list for processing
        // force user to place all pieces?
        // save game and peice locations
        // flip coin for who goes first

        // LoadGame index
        // Place pieces
        // Load history to replayed stack in order
        // Play out turns (if any) to get final locations
        // Load pieces to board


        // LoadGame
        // Find latest index (incomplete) and load that

        // database pieces - id, type
        // database turns - id, startPosition, endPosition, capture, team
        // database - game with id
        // piece postions - gameid, pieceid, position
    }
}
