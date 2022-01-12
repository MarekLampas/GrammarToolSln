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

        public Progress _ProgressNote { get; set; }

        public ObservableCollection<string> _Suggestions { get; set; }

        public bool _HasOutput { get; set; }

        public LL1Grammar(Symbols symbols, IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow, LL1ParsingTable? lL1ParsingTable, LL1WordParsing lL1WordParsing, LL1ParsingTree lL1ParsingTree, Progress progressNote, bool hasOutput)
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

            _ProgressNote = progressNote;

            _HasOutput = hasOutput;

            _Suggestions = new ObservableCollection<string>();
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
                        _ProgressNote.Note = $"First symbol on stack is Non-Terminal so we need to expand.\nNon-Terminal on stack is '{nonTerminal}' and Terminal on input is '{terminal}' and as we can see, there are multiple productions in this cell.\nTrerefor we can't decide if sentence belongs to our language.";
                        _ProgressNote.Color = Brushes.DarkRed;
                    }
                    else
                    {
                        if (productionEmpty.Count != 1)
                        {
                            _ProgressNote.Note = $"First symbol on stack is Non-Terminal so we need to expand.\nNon-Terminal on stack is '{nonTerminal}' and Terminal on input is '{terminal}'.\nBecause there is no production in this cell neither in cell for empty expansion '{LL1InputGrammar._EMPTY_EXPANSION}', we can assume that sentence does not belong our language.";
                            _ProgressNote.Color = Brushes.DarkRed;
                        }
                        else
                        {
                            _LL1WordParsing.Expand(productionEmpty.First());
                            _ProgressNote.Note = $"First symbol on stack is Non-Terminal so we need to expand.\nNon-Terminal on stack is '{nonTerminal}' and Terminal on input is '{terminal}'.\nBecause there is no production in this cell but there is possibility to use empty expansion '{LL1InputGrammar._EMPTY_EXPANSION}', we are aplying this production.";
                        }
                    }
                }
                else
                {
                    var outputSymbols = production.First().Split(' ').Where(x => x.StartsWith("[") && x.EndsWith("]"));

                    if (outputSymbols.Count() > 1)
                    {
                        _ProgressNote.Note = $"First symbol on stack is Non-Terminal so we need to expand.\nNon-Terminal on stack is '{nonTerminal}' and Terminal on input is '{terminal}' so we use production '{production.First()}' to expand our stack.\nBut production can't contain mote than one output symbol and this one contains {outputSymbols.Count()}.";
                        _ProgressNote.Color = Brushes.DarkRed;
                    }
                    else
                    {
                        var actualNode = _LL1WordParsing.GetParsingQueuedTreeSymbol();

                        _LL1WordParsing.Expand(production.First());

                        _ProgressNote.Note = $"First symbol on stack is Non-Terminal so we need to expand.\nNon-Terminal on stack is '{nonTerminal}' and Terminal on input is '{terminal}' so we use production '{production.First()}' to expand our stack.";

                        var prod = production.First().Split("->")[1].Trim();
                        var prodSplitted = prod.Split(" ");
                        for (int idx = 0; idx < prodSplitted.Length; idx++)
                        {
                            var sym = prodSplitted[prodSplitted.Length - idx - 1];
                            var nodeId = sym + _LL1ParsingTree.Nodes.Count.ToString();

                            if (!sym.StartsWith("[") && !sym.EndsWith("]"))
                            {
                                if (sym != LL1InputGrammar._EMPTY_EXPANSION)
                                    _LL1WordParsing._ParsingQueuedTree.Add(nodeId);

                                ISolidColorBrush background = _Symbols._NonTerminals.Contains(sym) ? Brushes.White : Brushes.LightSeaGreen;
                                _LL1ParsingTree.AddNode(nodeId, sym, background);

                                _LL1ParsingTree.AddEdge(actualNode, nodeId);
                            }
                        }
                    }
                }
            }
            else if (_Symbols._Terminals.Contains(_LL1WordParsing.GetParsingQueueSymbol()) || (_LL1WordParsing.GetParsingQueueSymbol() == LL1InputGrammar._END_STRING))
            {
                var terminalWord = _LL1WordParsing.GetRemaningWordSymbol();
                var terminalQueue = _LL1WordParsing.GetParsingQueueSymbol();

                if (terminalWord != terminalQueue)
                {
                    _ProgressNote.Note = $"First symbol on stack is Terminal so we need to consume.\nBut terminal on stack '{terminalQueue}' is not equal to terminal on input '{terminalWord}'.\nTherefore they can't be consumed and we can assume that sentance does not belong to out language.";
                    _ProgressNote.Color = Brushes.DarkRed;
                }
                else
                {
                    if (terminalWord == LL1InputGrammar._END_STRING)
                    {
                        _ProgressNote.Note = $"Neither input or stack contains any symbol.\nTherefore we can assume that sentance does belong to our language.";
                        _ProgressNote.Color = Brushes.DarkGreen;
                    }
                    else
                    {
                        _ProgressNote.Note = $"First symbol on stack is Terminal so we need to consume.\nWe can see that terminal on stack '{terminalQueue}' is equal to terminal on input '{terminalWord}'.\nTherefore they can be consumed.";
                        _LL1WordParsing.Consume();
                    }
                }
            }
            else if(_LL1WordParsing.GetParsingQueueSymbol().StartsWith("[") && _LL1WordParsing.GetParsingQueueSymbol().EndsWith("]"))
            {
                var terminalQueue = _LL1WordParsing.GetParsingQueueSymbol();

                _ProgressNote.Note = $"First symbol on stack is Output symbol so we need to add it's attribute to output.\nWe can see that terminal on stack '{terminalQueue}' so we must find it's corresponding atribute.";
                _LL1WordParsing.AddOutput();
            }
            else
            {
                _ProgressNote.Note = $"Symbol on stack '{_LL1WordParsing.GetParsingQueueSymbol()}' does not belong to Terminals neither Non-Terminals. Something went wrong.";
                _ProgressNote.Color = Brushes.DarkRed;
            }
        }

        //TODO: suggestions when entering rule
        public void GetSuggestions(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                List<string> suggestions = new List<string>();

                var wordArr = input.Split(" ");

                var lastWord = wordArr[wordArr.Length - 1];

                var possibleWords = _Symbols._Terminals.Where(x => x.StartsWith(lastWord));

                foreach (var possibleWord in possibleWords)
                {
                    wordArr[wordArr.Length - 1] = possibleWord;

                    suggestions.Add(string.Join(" ", wordArr));
                }

                _Suggestions = new ObservableCollection<string>(suggestions);
            }
        }
    }
}
