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
            // Load up specific starting pieces?
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
