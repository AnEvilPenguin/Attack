using Attack.Saves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Game
{
    internal class GameInstance
    {
        public DateTime? StartDate;
        public DateTime? CompletedDate;
        public DateTime? UpdateDate;

        public int Id;

        public string Player1 = "Hugh Mann";
        public string Player2 = "Robot Louis Stevenson";

        public string SaveName = $"Auto {DateTime.Now:f}";
    }
}
