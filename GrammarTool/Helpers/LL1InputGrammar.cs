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
        public const string _EMPTY_EXPANSION = "@";
        public const string _EMPTY_STRING = "";
        public const string _END_STRING = "$";

        public readonly Symbols _Symbols;
        
        public Dictionary<string, List<string>> _ProductionDict { get; private set; }

        public LL1InputGrammar(Symbols symbols, IEnumerable<LL1GrammarRule> rules)
        {
            _Symbols = symbols;

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
                var nonTerminalToProduction = rule.Rule.Split("->");

                nonTerminalToProduction[0] = nonTerminalToProduction[0].Trim();

                if (!_ProductionDict.ContainsKey(nonTerminalToProduction[0]))
                {
                    _Symbols.AddNonTerminal(nonTerminalToProduction[0]);

                    _ProductionDict.Add(nonTerminalToProduction[0], new List<string>());
                }

                _ProductionDict[nonTerminalToProduction[0]].AddRange(nonTerminalToProduction[1].Split(_RULES_SPLITTER).Select(x => x.Trim()));
            }

            if (!_Symbols._NonTerminals.Contains(LL1InputGrammar._STARTING_SYMBOL))
            {
                throw new Exception($"No rule for starting symbol {LL1InputGrammar._STARTING_SYMBOL} was provided");
            }

            foreach (var productionList in _ProductionDict.Values)
            {
                foreach (var production in productionList)
                {
                    foreach (var symbol in production.Split(" ").Select(x => x.Trim()))
                    {
                        if (!_Symbols._TokensUsed.Contains(symbol) && !_Symbols._NonTerminals.Contains(symbol) && (symbol != LL1InputGrammar._EMPTY_EXPANSION))
                        {
                            if (_Symbols._Terminals.Contains(symbol))
                            {
                                //throw new Exception($"Production left side contains symbol '{symbol}', but it's not used terminal or non terminal! But scanner terminals contains it!");
                            }
                            else
                            {
                                throw new Exception($"Production left side contains symbol '{symbol}', but it's not used terminal or non terminal!");
                            }
                        }
                    }
                }
            }
        }
    }
}
