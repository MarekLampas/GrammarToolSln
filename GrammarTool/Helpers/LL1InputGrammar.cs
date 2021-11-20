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

        public List<string> _Terminals { get; private set; }
        
        public List<string> _NonTerminals { get; private set; }
        
        public Dictionary<string, List<string>> _ProductionDict { get; private set; }

        public LL1InputGrammar(IEnumerable<LL1GrammarRule> rules)
        {
            Proccess(rules);
        }

        /// <summary>
        /// Asumes that non terminars are upper characters and everything else is terminal
        /// </summary>
        /// <param name="rules"></param>
        private void Proccess(IEnumerable<LL1GrammarRule> rules)
        {
            _Terminals = new List<string>();
            
            _NonTerminals = new List<string>();
            
            _ProductionDict = new Dictionary<string, List<string>>();

            foreach (var rule in rules)
            {
                var nonTerminalToProduction = rule.Rule.Split("->");
                
                if ((nonTerminalToProduction[0].Length > 1) || char.IsLower(nonTerminalToProduction[0][0]))
                    throw new Exception("Non terminal is not upper character.");

                if (!_ProductionDict.ContainsKey(nonTerminalToProduction[0]))
                {
                    _NonTerminals.Add(nonTerminalToProduction[0]);

                    _ProductionDict.Add(nonTerminalToProduction[0], new List<string>());
                }

                _ProductionDict[nonTerminalToProduction[0]].AddRange(nonTerminalToProduction[1].Split(_RULES_SPLITTER));
            }

            if (!_NonTerminals.Contains(LL1InputGrammar._STARTING_SYMBOL))
            {
                throw new Exception($"No rule for starting symbol {LL1InputGrammar._STARTING_SYMBOL} was provided");
            }

            foreach (var productionList in _ProductionDict.Values)
            {
                foreach (var production in productionList)
                {
                    foreach (var symbol in production.ToCharArray())
                    {
                        if (char.IsUpper(symbol))
                        {
                            if (!_NonTerminals.Contains(symbol.ToString()))
                            {
                                throw new Exception($"Production left side contains non terminal '{symbol}', but there's no produnction defined for it!");
                            }
                        }
                        else
                        {
                            _Terminals.Add(symbol.ToString());
                        }
                    }
                }
            }

            _Terminals = _Terminals.Distinct().ToList();
            _Terminals.Remove(_EMPTY_EXPANSION);
        }
    }
}
