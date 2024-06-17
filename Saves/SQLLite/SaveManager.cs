using Attack.Util;
using Microsoft.Data.Sqlite;
using Serilog;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Attack.Game;
using Godot;

namespace Attack.Saves.SQLLite
{
    internal class SaveManager : BaseSave
    {
        private readonly string _databasePath = $"{Constants.FolderPath}\\saves.db";

        private const string createGamesTableCommand =
            @"
                    CREATE TABLE 'Games' (
	                    'Id'	            INTEGER,
	                    'SaveName'	        TEXT,
	                    'Player1Name'	    TEXT,
	                    'Player2Name'	    TEXT,
	                    'StartDate'	        TEXT,
	                    'UpdateDate'	    TEXT,
	                    'CompletedDate'	    TEXT,
                        'StartingPlayer'    INTEGER,
	                    'Version'	        INTEGER NOT NULL DEFAULT 1,
	                    PRIMARY KEY('Id' AUTOINCREMENT)
                    );
            ";

        private const string createPositionsTableCommand =
            @"
                CREATE TABLE 'Positions' (
	                'Id'	    INTEGER NOT NULL,
	                'PieceId'	INTEGER NOT NULL,
	                'GameId'	INTEGER NOT NULL,
	                'StartX'	INTEGER NOT NULL,
	                'StartY'	INTEGER NOT NULL,
	                'Player'	INTEGER NOT NULL,
	                FOREIGN KEY('GameId') REFERENCES 'Games'('Id'),
	                PRIMARY KEY('Id' AUTOINCREMENT)
                );
            ";

        private const string createTurnsTableCommand =
            @"
                CREATE TABLE 'Turns' (
	                'Id'	        INTEGER NOT NULL,
	                'Game'	        INTEGER NOT NULL,
	                'StartX'	    INTEGER NOT NULL,
	                'StartY'	    INTEGER NOT NULL,
	                'EndX'	        INTEGER,
	                'EndY'	        INTEGER,
                    'AttackX'       INTEGER,
                    'AttackY'       INTEGER,
	                'DateTime'	    TEXT,
	                FOREIGN KEY('Game') REFERENCES 'Games'('Id'),
	                PRIMARY KEY('Id' AUTOINCREMENT)
                );
            ";

        private SaveGame _saveGame;
        private SaveTurn _saveTurn;

        public SaveManager()
        {
            Log.Debug("SqlLiteSaveManager Constructed");
            _connectionString = $"Data Source={_databasePath}";

            _saveGame = new SaveGame();
            _saveTurn = new SaveTurn();
        }

        public GameInstance NewGame() =>
            _saveGame.Create();
        
        public void SaveGame(GameInstance game) =>
            _saveGame.Save(game);

        public void DeleteGame(GameInstance game) =>
            _saveGame.Delete(game);

        public GameInstance LoadGame(int id) =>
            _saveGame.Load(id);

        public List<GameInstance> ListLoadableGames() =>
            _saveGame.GetAll()
                .Where(g => g.CompletedDate == null)
                .ToList();

        public void SaveGame(GameInstance game, Turn turn)
        {
            _saveTurn.Save(game, turn);
            _saveGame.Save(game);
        }

        public int GetLatestSaveId() =>
            _saveGame.GetLatestId();

        public Queue<Vector2I[]> LoadTurns(GameInstance game) =>
            _saveTurn.Load(game);

        public List<PieceNode> LoadPieces(GameInstance game) =>
            _saveGame.LoadPieces(game);


        public void Initialize()
        {
            Log.Debug("Initializing database");

            if (File.Exists(_databasePath))
            {
                Log.Debug("Existing database file found");
                // TODO consider version checks, upgrade paths, etc.

                return;
            }

            using (var connection = new SqliteConnection(_connectionString))
            {
                Log.Debug("Connecting to database");
                connection.Open();

                createTable(connection, "Games", createGamesTableCommand);
                createTable(connection, "Positions", createPositionsTableCommand);
                createTable(connection, "Turns", createTurnsTableCommand);

                Log.Debug("Closing connection to database");
            }

            Log.Debug("Database initialized");
        }

        private void createTable(SqliteConnection connection, string tableName, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;

            try
            {
                command.ExecuteNonQuery();

                Log.Debug($"Created {tableName} Table");
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Failed to create {tableName} Table");
                // TODO consider what we want to do in these scenarios
                // Running without saves is probably acceptable
            }
        }
    }
}
