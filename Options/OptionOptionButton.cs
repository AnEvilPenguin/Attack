using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Options
{
    internal partial class OptionOptionButton: OptionButton
    {
        public OptionOptionButton(string[] items, string selectedItem)
        {
            foreach (var item in items) 
                AddItem(item);

            int index = items
                .Select((item, index) => new { item, index })
                .Where(i => i.item == selectedItem)
                .Select(i => i.index)
                .First();

            Select(index);
        }
    }
}
