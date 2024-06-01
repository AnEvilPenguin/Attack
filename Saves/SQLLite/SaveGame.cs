using Attack.Game;
using Attack.Util;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Saves.SQLLite
{
    internal class SaveGame : BaseSave
    {
        private readonly string _connectionString;

        public SaveGame(string connectionString)
        {
        }

        public GameInstance Create()
        {
            Log.Information("Creating game instance");

            GameInstance instance = new GameInstance();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                command.CommandText = "INSERT INTO Games DEFAULT VALUES;";
                command.ExecuteNonQuery();

                command.CommandText = "SELECT last_insert_rowid()";
                long newId = (long)command.ExecuteScalar();

                Log.Debug($"Created Game Instance with Id: {newId}");

                instance.Id = (int)newId;
            }

            return instance;
        }

        public void Delete(GameInstance game)
        {
            Log.Warning($"Deleting game instance {game.Id}");

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        DELETE FROM Games
                        WHERE Id = $Id
                    ";

                command.Parameters.AddWithValue("$Id", game.Id);

                try
                {
                    command.ExecuteNonQuery();

                    Log.Debug($"Successfully deleted game {game.Id}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to delete game {game.Id}");
                    throw;
                }
            }
        }

        public GameInstance Load(int id)
        {
            Log.Information($"Loading game instance {id}");

            GameInstance instance = new GameInstance();
            SqliteDataReader reader;

            using (var connection = new SqliteConnection( _connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        SELECT * FROM Games
                        WHERE Id = $Id
                    ";

                command.Parameters.AddWithValue("$Id", id);

                reader = command.ExecuteReader();

                if (!reader.HasRows)
                {
                    string message = $"No records with Id {id}";

                    Log.Error(message);
                    throw new IndexOutOfRangeException(message);
                }

                while (reader.Read())
                {
                    instance.Id = (int)(long)reader.GetValue(0);

                    instance.SaveName = (string)reader.GetValue(1);
                    instance.Player1 = (string)reader.GetValue(2);
                    instance.Player2 = (string)reader.GetValue(3);

                    instance.UpdateDate = DateTime.Parse((string)reader.GetValue(5));

                    var startDate = reader.GetValue(4);

                    if (startDate is not DBNull)
                        instance.StartDate = DateTime.Parse(startDate.ToString());

                    var endDate = reader.GetValue(6);

                    if (endDate is not DBNull)
                        instance.CompletedDate = DateTime.Parse(endDate.ToString());
                }
            }

            return instance;
        }

        public void Save(GameInstance game)
        {
            Log.Information($"Saving game instance {game.Id}");

            using (var connection = new SqliteConnection( _connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        UPDATE Games
                        SET SaveName = $saveName,
                            Player1Name = $player1Name,
                            Player2Name = $player2Name,
                            StartDate = $startDate,
                            UpdateDate = $updateDate,
                            CompletedDate = $completedDate
                        WHERE Id = $Id
                    ";

                var now = DateTime.UtcNow;

                command.Parameters.AddWithValue("$saveName", game.SaveName);

                command.Parameters.AddWithValue("$player1Name", game.Player1);
                command.Parameters.AddWithValue("$player2Name", game.Player2);

                if (game.StartDate == null)
                    command.Parameters.AddWithValue("$startDate", DBNull.Value);
                else
                    command.Parameters.AddWithValue("$startDate", game.StartDate?.ToString(Constants.DateStringFormat));

                if (game.CompletedDate == null)
                    command.Parameters.AddWithValue("$completedDate", DBNull.Value);
                else
                    command.Parameters.AddWithValue("$completedDate", game.CompletedDate?.ToString(Constants.DateStringFormat));

                command.Parameters.AddWithValue("$updateDate", now.ToString(Constants.DateStringFormat));

                command.Parameters.AddWithValue("$Id", game.Id);

                try
                {
                    command.ExecuteNonQuery();

                    Log.Debug($"Successfully saved game {game.Id}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to save game {game.Id}");
                    throw;
                }
            }
        }

        public List<GameInstance> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
