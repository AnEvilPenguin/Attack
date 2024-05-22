using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Attack.Options
{
    internal partial class OptionCheckButton: CheckButton
    {
        public OptionCheckButton(string text, Action pressed, bool selected)
        {
            Text = text;
            Pressed += pressed;
            ButtonPressed = selected;
        }
    }
}
