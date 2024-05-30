using Attack.Saves.SQLLite;
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
        private static bool _initialized = false;

        // private stack previous turns
        // private stack replayed

        private static SaveManager _sqlSaveManager;

        private static GameInstance _gameInstance;

        public bool GameStarted = false;

        private static BoardMap _board;

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
            try
            {
                _gameInstance = _sqlSaveManager.NewGame();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Failed to create new game");
            }

            _sqlSaveManager.SaveGame(_gameInstance);

            GetTree().ChangeSceneToFile("res://game.tscn");
        }

        public void CreateGame(BoardMap board)
        {
            _board = board;

            foreach (var pieceConfig in Presets.StandardSetup.Locations)
            {
                var location = pieceConfig.Item1;
                var pieceType = pieceConfig.Item2;

                _board.createPiece(location, pieceType, Team.Red);
            }
        }

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
