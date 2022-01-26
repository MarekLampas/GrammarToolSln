using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Helpers
{
    public class LL1InputGrammar
    {
        //special characters for grammar processing that can't be user as grammar symbols
        public const string _STARTING_SYMBOL = "S";
        public const string _RULES_SPLITTER = "/";
        public const string _EMPTY_EXPANSION = "ε";
        public const string _EMPTY_EXPANSION_INSERT = "epsilon";
        public const string _EMPTY_STRING = "";
        public const string _END_STRING = "$";

        public readonly Symbols _Symbols;
        
        public Dictionary<string, List<string>> _ProductionDict { get; private set; }

        public bool _HasOutput { get; set; }

        public string _Error { get; set; }

        public LL1InputGrammar(Symbols symbols, IEnumerable<LL1GrammarRule> rules)
        {
            _Symbols = symbols;

            _Symbols._NonTerminals.Clear();

            _HasOutput = false;

            _Error = string.Empty;

            Proccess(rules);
        }

        /// <summary>
        /// Asumes that only non terminars are on left sides of rules and all terminals are already known
        /// </summary>
        /// <param name="rules"></param>
        private void Proccess(IEnumerable<LL1GrammarRule> rules)
        {
            _ProductionDict = new Dictionary<string, List<string>>();

            foreach (var rule in rules)
            {
                rule.Rule.Replace(LL1InputGrammar._EMPTY_EXPANSION_INSERT, LL1InputGrammar._EMPTY_EXPANSION);

                var nonTerminalToProduction = rule.Rule.Split("->");

                nonTerminalToProduction[0] = nonTerminalToProduction[0].Trim();

                if (nonTerminalToProduction[0].Split(" ").Length > 1)
                {
                    _Error = $"Rule's left side must be single symbol.";
                    return;
                }

                if (_Symbols._Terminals.Contains(nonTerminalToProduction[0]))
                {
                    _Error = $"Rule's left side can't contain symbol {nonTerminalToProduction[0]} because it's terminal symbol.";
                    return;
                }

                if (!_ProductionDict.ContainsKey(nonTerminalToProduction[0]))
                {
                    _Symbols.AddNonTerminal(nonTerminalToProduction[0]);

                    _ProductionDict.Add(nonTerminalToProduction[0], new List<string>());
                }

                _ProductionDict[nonTerminalToProduction[0]].AddRange(nonTerminalToProduction[1].Split(_RULES_SPLITTER).Select(x => x.Trim()));
            }

            if (!_Symbols._NonTerminals.Contains(LL1InputGrammar._STARTING_SYMBOL))
            {
                _Error = $"No rule for starting symbol {LL1InputGrammar._STARTING_SYMBOL} was provided";
            }

            foreach (var productionList in _ProductionDict.Values)
            {
                foreach (var production in productionList)
                {
                    var outputSymbols = production.Split(' ').Where(x => x.StartsWith("[") && x.EndsWith("]"));

                    if (outputSymbols.Count() > 1)
                    {
                        _Error = $"Production can't contain mote than one output symbol but production {production} contains {outputSymbols.Count()}.";
                        return;
                    }

                    foreach (var symbol in production.Split(" ").Select(x => x.Trim()))
                    {
                        if (!_Symbols._TokensUsed.Contains(symbol) && !_Symbols._NonTerminals.Contains(symbol) && (symbol != LL1InputGrammar._EMPTY_EXPANSION))
                        {
                            if (_Symbols._Terminals.Contains(symbol))
                            {
                                _Symbols._TokensUsed.Add(symbol);
                                //throw new Exception($"Production left side contains symbol '{symbol}', but it's not used terminal or non terminal! But scanner terminals contains it!");
                            }
                            else
                            {
                                if (!symbol.StartsWith("[") || !symbol.EndsWith("]") || !_Symbols._TokensUsed.Contains(symbol.Substring(1, symbol.Length - 2)))
                                {
                                    _Error = $"Production {production} contains symbol '{symbol}', but it's not used terminal or non terminal!";
                                    return;
                                }
                                else
                                {
                                    _HasOutput = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
