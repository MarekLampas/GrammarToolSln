using Avalonia.Controls;
using Avalonia.Data;
using GrammarTool.ViewModels;
using GrammarTool.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class LL1Grammar
    {
        public ObservableCollection<LL1GrammarRule> _LL1Rules { get; }

        public ObservableCollection<LL1FirstFollow> _LL1FirstFollow { get; set; }

        public ObservableCollection<LL1FirstByProduction> _FirstSetByProduction { get; set; }

        public LL1ParsingTable _LL1ParsingTable { get; set; }

        public LL1Grammar(IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow)
        {
            _LL1Rules = new ObservableCollection<LL1GrammarRule>(rules);

            _LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(firstFollow);

            List<LL1FirstByProduction> firstSetByProductionInit = new List<LL1FirstByProduction>();

            foreach (var firstSetByProductionDict in _LL1FirstFollow.Select(x => x._FirstSetByProduction))
                foreach (var firstSetByProduction in firstSetByProductionDict)
                    firstSetByProductionInit.Add(new LL1FirstByProduction(firstSetByProduction.Key, string.Join(", ", firstSetByProduction.Value)));

            _FirstSetByProduction = new ObservableCollection<LL1FirstByProduction>(firstSetByProductionInit);

            _LL1ParsingTable = new LL1ParsingTable(_LL1FirstFollow);
        }
    }
}
