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

namespace Attack.Saves.SQLLite
{
    internal class SaveManager
    {
        private readonly string _databasePath = $"{Constants.FolderPath}\\saves.db";
        private readonly string _connectionString;

        private const string createGamesTableCommand =
            @"
                    CREATE TABLE 'Games' (
	                    'Id'	        INTEGER,
	                    'SaveName'	    TEXT,
	                    'Player1Name'	TEXT,
	                    'Player2Name'	TEXT,
	                    'StartDate'	    TEXT,
	                    'UpdateDate'	TEXT,
	                    'CompletedDate'	TEXT,
	                    'Version'	    INTEGER NOT NULL DEFAULT 1,
	                    PRIMARY KEY('Id' AUTOINCREMENT)
                    );
            ";

        private const string createPiecesTableCommand =
            @"
                CREATE TABLE 'Pieces' (
	                'Id'	INTEGER NOT NULL,
	                'Type'	INTEGER NOT NULL,
	                PRIMARY KEY('Id' AUTOINCREMENT)
                );
            ";

        private const string createPositionsTableCommand =
            @"
                CREATE TABLE 'Positions' (
	                'Id'	INTEGER NOT NULL,
	                'PieceId'	INTEGER NOT NULL,
	                'GameId'	INTEGER NOT NULL,
	                'StartX'	INTEGER NOT NULL,
	                'StartY'	INTEGER NOT NULL,
	                'Player'	INTEGER NOT NULL,
	                FOREIGN KEY('PieceId') REFERENCES 'Pieces'('Id'),
	                FOREIGN KEY('GameId') REFERENCES 'Games'('Id'),
	                PRIMARY KEY('Id' AUTOINCREMENT)
                );
            ";

        private const string createTurnsTableCommand =
            @"
                CREATE TABLE 'Turns' (
	                'Id'	INTEGER NOT NULL,
	                'Piece'	INTEGER NOT NULL,
	                'Game'	INTEGER NOT NULL,
	                'StartX'	INTEGER NOT NULL,
	                'StartY'	INTEGER NOT NULL,
	                'ExdX'	INTEGER,
	                'EndY'	INTEGER,
	                'Capture'	INTEGER,
	                'TurnNumber'	INTEGER,
	                'DateTime'	TEXT,
	                FOREIGN KEY('Piece') REFERENCES 'Pieces'('Id'),
	                FOREIGN KEY('Capture') REFERENCES 'Pieces'('Id'),
	                FOREIGN KEY('Game') REFERENCES 'Games'('Id'),
	                PRIMARY KEY('Id' AUTOINCREMENT)
                );
            ";

        private SaveGame _saveGame;

        public SaveManager()
        {
            Log.Debug("SqlLiteSaveManager Constructed");
            _connectionString = $"Data Source={_databasePath}";

            _saveGame = new SaveGame(_connectionString);
        }

        public GameInstance NewGame() =>
            _saveGame.Create();
        
        public void SaveGame(GameInstance game) =>
            _saveGame.Save(game);

        public void DeleteGame(GameInstance game) =>
            _saveGame.Delete(game);

        public GameInstance LoadGame(int id) =>
            _saveGame.Load(id);

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
                createTable(connection, "Pieces", createPiecesTableCommand);
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
