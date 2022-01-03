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
using Avalonia.Media;

namespace GrammarTool.Models
{
    public class LL1Grammar
    {
        public Symbols _Symbols { get; }

        public ObservableCollection<LL1GrammarRule> _LL1Rules { get; set; }

        public ObservableCollection<LL1FirstFollow> _LL1FirstFollow { get; set; }

        public ObservableCollection<LL1FirstByProduction> _FirstSetByProduction { get; set; }

        public LL1ParsingTable _LL1ParsingTable { get; set; }

        public LL1WordParsing _LL1WordParsing { get; set; }

        public LL1ParsingTree _LL1ParsingTree { get; set; }

        public LL1Grammar(Symbols symbols, IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow, LL1ParsingTable? lL1ParsingTable, LL1WordParsing lL1WordParsing, LL1ParsingTree lL1ParsingTree)
        {
            _Symbols = symbols;

            //TODO: verify if rules do not contains multiple whitespaces in a row and get rid of all Trim() random in code
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

            _LL1ParsingTree = lL1ParsingTree;
        }

        public void DoParseStep()
        {
            if (_Symbols._NonTerminals.Contains(_LL1WordParsing.GetParsingQueueSymbol()))
            {
                var nonTerminal = _LL1WordParsing.GetParsingQueueSymbol();
                var terminal = _LL1WordParsing.GetRemaningWordSymbol();

                if (terminal == LL1InputGrammar._END_STRING)
                    terminal = LL1InputGrammar._EMPTY_EXPANSION;

                var production = _LL1ParsingTable._ParsingTable[nonTerminal][terminal];

                if (production.Count != 1)
                {
                    var productionEmpty = _LL1ParsingTable._ParsingTable[nonTerminal][LL1InputGrammar._EMPTY_EXPANSION];
                    if (production.Count > 1)
                    {
                        throw new Exception($"Parsing table cell ({nonTerminal}, {terminal}) does not contain exact one production.");
                    }
                    else
                    {
                        if (productionEmpty.Count != 1)
                        {
                            throw new Exception($"Parsing table cell ({nonTerminal}, {terminal}) neiter cell ({nonTerminal}, {LL1InputGrammar._EMPTY_EXPANSION}) does contain exact one production.");
                        }
                        else
                        {
                            _LL1WordParsing.Expand(productionEmpty.First());
                        }
                    }
                }
                else
                {
                    var actualNode = _LL1WordParsing.GetParsingQueuedTreeSymbol();

                    _LL1WordParsing.Expand(production.First());

                    var prod = production.First().Split("->")[1].Trim();
                    var prodSplitted = prod.Split(" ");
                    for (int idx = 0; idx < prodSplitted.Length; idx++)
                    {
                        var sym = prodSplitted[prodSplitted.Length - idx - 1];
                        var nodeId = sym + _LL1ParsingTree.Nodes.Count.ToString();

                        if(sym != LL1InputGrammar._EMPTY_EXPANSION)
                            _LL1WordParsing._ParsingQueuedTree.Add(nodeId);

                        ISolidColorBrush background = _Symbols._NonTerminals.Contains(sym) ? Brushes.White : Brushes.LightSeaGreen;
                        _LL1ParsingTree.AddNode(nodeId, sym, background);

                        _LL1ParsingTree.AddEdge(actualNode, nodeId);
                    }
                }
            }
            else if (_Symbols._Terminals.Contains(_LL1WordParsing.GetParsingQueueSymbol()) || (_LL1WordParsing.GetParsingQueueSymbol() == LL1InputGrammar._END_STRING))
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
