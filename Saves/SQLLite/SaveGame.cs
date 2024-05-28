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
    internal class SaveGame
    {
        private readonly string _connectionString;

        public SaveGame(string connectionString)
        {
            _connectionString = connectionString;
        }

        public GameInstance Create()
        {
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
            throw new NotImplementedException();
        }

        public void Save(GameInstance game)
        {
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
