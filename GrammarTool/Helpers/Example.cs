using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Helpers
{
    [Serializable]
    public class Example
    {
        public int? SelectedIndex;

        public bool[]? IsChecked;

        public string? InputText;

        public LL1GrammarRule[]? Rules;

        public Example()
        {
        }

        public Example(int selectedIndex, bool[] isChecked, string inputText, LL1GrammarRule[] rules)
        {
            SelectedIndex = selectedIndex;

            IsChecked = isChecked;

            InputText = inputText;

            Rules = rules;
        }
    }
}
