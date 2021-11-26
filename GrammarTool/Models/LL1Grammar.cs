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

        public LL1WordParsing _LL1WordParsing { get; set; }

        public LL1Grammar(IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow, LL1ParsingTable? lL1ParsingTable, LL1WordParsing lL1WordParsing)
        {
            _LL1Rules = new ObservableCollection<LL1GrammarRule>(rules);

            _LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(firstFollow);

            List<LL1FirstByProduction> firstSetByProductionInit = new List<LL1FirstByProduction>();

            foreach (var firstSetByProductionDict in _LL1FirstFollow.Select(x => x._FirstSetByProduction))
                foreach (var firstSetByProduction in firstSetByProductionDict)
                    firstSetByProductionInit.Add(new LL1FirstByProduction(firstSetByProduction.Key, string.Join(", ", firstSetByProduction.Value)));

            _FirstSetByProduction = new ObservableCollection<LL1FirstByProduction>(firstSetByProductionInit);

            if (lL1ParsingTable == null)
            {
                _LL1ParsingTable = new LL1ParsingTable(_LL1FirstFollow);
            }
            else
            {
                if (lL1ParsingTable._LL1TerminalToProductions.Count < 1)
                {
                    _LL1ParsingTable = new LL1ParsingTable(_LL1FirstFollow);
                }
                else
                {
                    _LL1ParsingTable = lL1ParsingTable;
                }
            }

            _LL1WordParsing = lL1WordParsing;
        }

        public void DoParseStep()
        {
            if (_LL1FirstFollow.First()._LL1InputGrammar._NonTerminals.Contains(_LL1WordParsing.GetParsingQueueSymbol()))
            {
                var nonTerminal = _LL1WordParsing.GetParsingQueueSymbol();
                var terminal = _LL1WordParsing.GetRemaningWordSymbol();
                var production = _LL1ParsingTable._ParsingTable[nonTerminal][terminal];

                if (production.Count != 1)
                {
                    var productionEmpty = _LL1ParsingTable._ParsingTable[nonTerminal][LL1InputGrammar._EMPTY_EXPANSION];
                    if (productionEmpty.Count != 1)
                    {
                        throw new Exception($"Parsing table cell ({nonTerminal}, {terminal}) does not contains exact one production.");
                    }
                    else
                    {
                        _LL1WordParsing.Expand(productionEmpty.First());
                    }
                }
                else
                {
                _LL1WordParsing.Expand(production.First());
                }
            }
            else if (_LL1FirstFollow.First()._LL1InputGrammar._Terminals.Contains(_LL1WordParsing.GetParsingQueueSymbol()) || (_LL1WordParsing.GetParsingQueueSymbol() == LL1InputGrammar._END_STRING))
            {
                var terminalWord = _LL1WordParsing.GetRemaningWordSymbol();
                var terminalQueue = _LL1WordParsing.GetParsingQueueSymbol();

                if (terminalWord != terminalQueue)
                {
                    throw new Exception($"Next terminal in word ({terminalWord}) is not equal to next terminal in stack ({terminalQueue}).");
                }
                else
                {
                    _LL1WordParsing.Consume();
                }
            }
            else
            {
                throw new Exception($"There was unexpected symbol in parsing stack ({_LL1WordParsing.GetParsingQueueSymbol()}).");
            }
        }
    }
}
