using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class LL1ParsingTable
    {
        //TODO: try to use class instead of string array
        public ObservableCollection<string[]> _LL1TerminalToProductions { get; set; }

        public Dictionary<string, Dictionary<string, HashSet<string>>> _ParsingTable;

        public LL1ParsingTable(ObservableCollection<LL1FirstFollow> LL1FirstFollow)
        {
            List<string[]> lL1TerminalToProductions = new List<string[]>();

            if (LL1FirstFollow.Count > 0)
            {
                List<string> row = new List<string>();

                row.Add(string.Empty);

                row.AddRange(LL1FirstFollow[0]._TerminalToProduction.Keys);

                lL1TerminalToProductions.Add(row.ToArray());

                _ParsingTable = new Dictionary<string, Dictionary<string, HashSet<string>>>();

                foreach (var firstFollow in LL1FirstFollow)
                {
                    row.Clear();

                    row.Add(firstFollow._NonTerminal);

                    row.AddRange(firstFollow._TerminalToProduction.Select(x => string.Join(", ", x.Value.Select(y => y.Split("->")[1].Trim()))).ToArray());

                    lL1TerminalToProductions.Add(row.ToArray());

                    var parsingTable = new Dictionary<string, HashSet<string>>();

                    foreach (var terminalToProduction in firstFollow._TerminalToProduction)
                    {
                        parsingTable.Add(terminalToProduction.Key, terminalToProduction.Value);
                    }

                    _ParsingTable.Add(firstFollow._NonTerminal, parsingTable);
                }
            }

            _LL1TerminalToProductions = new ObservableCollection<string[]>(lL1TerminalToProductions);
        }
    }
}
