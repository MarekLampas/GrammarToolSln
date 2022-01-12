using Avalonia.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Helpers
{
    public class Progress
    {
        public string Note { get; set; }

        public ISolidColorBrush Color { get; set; }

        public Progress()
        {
            Note = string.Empty;

            Color = Brushes.White;
        }
    }
}
