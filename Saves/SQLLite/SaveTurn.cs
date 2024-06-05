using Attack.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Attack.Util;
using Serilog;
using Godot;

namespace Attack.Saves.SQLLite
{
    internal class SaveTurn : BaseSave
    {

        public SaveTurn() { }

        public void Save(GameInstance game, Turn turn)
        {
            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();
                var parameters = command.Parameters;

                command.CommandText =
                    @"
                        INSERT INTO Turns (Game, StartX, StartY, EndX, EndY, AttackX, AttackY, DateTime)
                        VALUES($gameId, $startX, $startY, $endX, $endY, $attackX, $attackY, $dateTime)
                    ";

                parameters.AddWithValue("$gameId", game.Id);

                var startPosition = turn.SelectedTile.Position;

                parameters.AddWithValue("$startX", startPosition.X);
                parameters.AddWithValue("$startY", startPosition.Y);

                if (turn.DestinationTile == null) 
                {
                    parameters.AddWithValue("$endX", DBNull.Value);
                    parameters.AddWithValue("$endY", DBNull.Value);
                }
                else
                {
                    var endPosition = turn.DestinationTile.Position;

                    parameters.AddWithValue("$endX", endPosition.X);
                    parameters.AddWithValue("$endY", endPosition.Y);
                }

                if (turn.AttackedTile == null)
                {
                    parameters.AddWithValue("$attackX", DBNull.Value);
                    parameters.AddWithValue("$attackY", DBNull.Value);
                }
                else
                {
                    var attackPosition = turn.AttackedTile.Position;

                    parameters.AddWithValue("$attackX", attackPosition.X);
                    parameters.AddWithValue("$attackY", attackPosition.Y);
                }

                var now = DateTime.UtcNow;

                parameters.AddWithValue("$dateTime", now.ToString(Constants.DateStringFormat));

                try
                {
                    command.ExecuteNonQuery();

                    Log.Debug($"Successfully saved turn for {game.Id}");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to save turn for {game.Id}");
                    throw;
                }
            }
        }

        public Queue<Vector2I[]> Load(GameInstance game)
        {
            var queue = new Queue<Vector2I[]>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        SELECT * FROM Turns
                        WHERE GameId = $Id
                        ORDER BY Id
                    ";

                command.Parameters.AddWithValue("$Id", game.Id);

                SqliteDataReader reader;

                try
                {
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to read game turns for {game.Id}");
                    throw;
                }

                while (reader.Read())
                {
                    var arr = new Vector2I[2];

                    int startX = (int)(float)reader.GetValue(2);
                    int startY = (int)(float)reader.GetValue(3);

                    arr[0] = new Vector2I(startX, startY);

                    // FIXME These may not exist
                    var endX = (int)(float)reader.GetValue(4);
                    var endY = (int)(float)reader.GetValue(5);

                    arr[1] = new Vector2I(endX, endY);

                    var attackX = (int)(float)reader.GetValue(6);
                    var attackY = (int)(float)reader.GetValue(7);

                    arr[2] = new Vector2I(attackX, attackY);

                    queue.Enqueue(arr);
                }
            }

            return queue;
        }
    }
}
