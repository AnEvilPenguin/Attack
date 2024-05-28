using Attack.Game;
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

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public GameInstance Load(int id)
        {
            throw new NotImplementedException();
        }

        public void Save(GameInstance game)
        {
            throw new NotImplementedException();
        }

        public List<GameInstance> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}
