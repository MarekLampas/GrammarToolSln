using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.ViewModels
{
    class GrammarPanelViewModel : ViewModelBase
    {
        public GrammarPanelViewModel(IEnumerable<GrammarRule> items)
        {
            Items = new ObservableCollection<GrammarRule>(items);
        }

        public ObservableCollection<GrammarRule> Items { get; }
    }
}
