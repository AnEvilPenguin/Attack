using Attack.Game;
using Godot;
using Microsoft.Data.Sqlite;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Godot.WebSocketPeer;

namespace Attack.Saves.SQLLite
{
    internal class SaveStartingLocations : BaseSave
    {
        public SaveStartingLocations() { }

        public bool HasStartLocations(GameInstance game)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText = @"SELECT EXISTS (SELECT Id FROM Positions WHERE GameId = $id) as Result;";

                command.Parameters.AddWithValue("$id", game.Id);

                try
                {
                    var result = command.ExecuteScalar();

                    Log.Debug($"Successfully queried for game");

                    return Convert.ToBoolean(result);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to save location {game.Id}");
                    throw;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public void Save(GameInstance game)
        {

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        INSERT INTO Positions (PieceId, GameId, StartX, StartY, Player)
                        VALUES($pieceId, $gameId, $startX, $startY, $player);
                    ";

                foreach (var tile in game.StartingPositions)
                {
                    command.Parameters.Clear();

                    var piece = tile.Piece;
                    var position = tile.Position;

                    command.Parameters.AddWithValue("$pieceId", (int)piece.PieceType);
                    command.Parameters.AddWithValue("$gameId", game.Id);
                    command.Parameters.AddWithValue("$startX", position.X);
                    command.Parameters.AddWithValue("$startY", position.Y);
                    command.Parameters.AddWithValue("$player", (int)piece.Team);

                    try
                    {
                        command.ExecuteNonQuery();

                        Log.Debug($"Successfully saved location {piece.PieceType}:{piece.Team} - {game.Id}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Failed to save location {game.Id}");
                        throw;
                    }
                }
            }
        }

        public List<PieceNode> Load(GameInstance game)
        {
            Log.Debug($"Loading initial piece placement for {game.Id}");

            var list = new List<PieceNode>();

            using (var connection = new SqliteConnection(_connectionString))
            {
                connection.Open();

                var command = connection.CreateCommand();

                command.CommandText =
                    @"
                        SELECT * FROM Positions
                        WHERE GameId = $gameId;
                    ";

                command.Parameters.AddWithValue("$gameId", game.Id);

                SqliteDataReader reader;

                try
                {
                    reader = command.ExecuteReader();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, $"Failed to read placement for {game.Id}");
                    throw;
                }

                while (reader.Read())
                {
                    int id = (int)(long)reader.GetValue(0);

                    Log.Debug($"Loading placement {id} of game {game.Id}");

                    int pieceId = (int)(long)reader.GetValue(1);

                    int startX = (int)(long)reader.GetValue(3);
                    int startY = (int)(long)reader.GetValue(4);

                    int team = (int)(long)reader.GetValue(5);

                    var piece = new PieceNode() // Bodge to smuggle piece to tile
                    {
                        PieceType = (PieceType)pieceId,
                        Position = new Vector2I(startX, startY), 
                        Team = (Team)team
                    };

                    Log.Debug($"Piece is {piece.PieceType} at {piece.Position} for {piece.Team}");

                    list.Add(piece);
                }
            }

            return list;
        }
    }
}
