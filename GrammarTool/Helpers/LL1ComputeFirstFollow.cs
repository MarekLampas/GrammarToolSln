using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrammarTool.Models;

namespace GrammarTool.Helpers
{
    public class LL1ComputeFirstFollow
    {
        public readonly Symbols _Symbols;

        public readonly LL1InputGrammar _LL1InputGrammar;

        public Dictionary<string, HashSet<string>> _First;

        Dictionary<string, HashSet<string>> _FirstByRule;

        public Dictionary<string, HashSet<string>> _Follow;

        public LL1ComputeFirstFollow(Symbols symbols, LL1InputGrammar lL1InputGrammar)
        {
            _Symbols = symbols;

            _LL1InputGrammar = lL1InputGrammar;

            _First = new Dictionary<string, HashSet<string>>();

            _FirstByRule = new Dictionary<string, HashSet<string>>();

            _Follow = new Dictionary<string, HashSet<string>>();
        }

        public IEnumerable<LL1FirstFollow> Compute(IEnumerable<LL1GrammarRule> rules)
        {
            List<LL1FirstFollow> lL1FirstFollow = new List<LL1FirstFollow>();

            foreach (var nonTerminal in _Symbols._NonTerminals)
            {
                _First.Add(nonTerminal, new HashSet<string>());
                _Follow.Add(nonTerminal, new HashSet<string>());
            }

            foreach (var nonTerminalRules in _LL1InputGrammar._ProductionDict)
            {
                var individualRules = new List<string>();

                individualRules.AddRange(nonTerminalRules.Value.Select(x => $"{nonTerminalRules.Key} -> {x}"));

                foreach (var individualRule in individualRules)
                {
                    _FirstByRule.Add(individualRule, new HashSet<string>());
                }
            }

            while (true)
            {
                Dictionary<string, HashSet<string>> firstByRuleOld = new Dictionary<string, HashSet<string>>();
                
                foreach (var firstByRule in _FirstByRule)
                {
                    firstByRuleOld.Add(firstByRule.Key, firstByRule.Value.Select(x => x).ToHashSet());
                }
                
                bool wasChanged = false;

                foreach (var rule in _FirstByRule.Keys)
                {
                    if (rule.StartsWith("[") && rule.EndsWith("]"))
                    {
                        First(string.Join(" ", rule.Split(" ").Take(rule.Split(" ").Length)));
                    }
                    else
                    {
                        First(rule);
                    }
                    
                    if (!(_FirstByRule[rule].SetEquals(firstByRuleOld[rule])))
                        wasChanged = true;
                }

                if (!wasChanged)
                    break;
            }

            _Follow[LL1InputGrammar._STARTING_SYMBOL].Add(LL1InputGrammar._END_STRING);

            while (true)
            {
                Dictionary<string, HashSet<string>> _followOld = new Dictionary<string, HashSet<string>>();

                foreach (var nonTerminal in _Symbols._NonTerminals)
                {
                    _followOld.Add(nonTerminal, _Follow[nonTerminal].Select(x => x).ToHashSet());
                }

                bool wasChanged = false;

                foreach (var rule in _FirstByRule.Keys)
                {
                    if (rule.StartsWith("[") && rule.EndsWith("]"))
                    {
                        Follow(string.Join(" ", rule.Split(" ").Take(rule.Split(" ").Length)));
                    }
                    else
                    {
                        Follow(rule);
                    }
                }

                foreach (var nonTerminal in _Follow.Keys)
                {
                    if (!(_Follow[nonTerminal].SetEquals(_followOld[nonTerminal])))
                        wasChanged = true;
                }

                if (!wasChanged)
                    break;
            }

            foreach(var nonTerminal in _Symbols._NonTerminals)
            {
                Dictionary<string, HashSet<string>> _FirstByRuleOfNonTerminal = new Dictionary<string, HashSet<string>>();

                foreach(var firstByRule in _FirstByRule)
                {
                    if (_LL1InputGrammar._ProductionDict[nonTerminal].Contains(firstByRule.Key.Split("->")[1].Trim()) && (firstByRule.Key.Split("->")[0].Trim() == nonTerminal))
                    {
                        _FirstByRuleOfNonTerminal.Add(firstByRule.Key, firstByRule.Value);
                    }
                }

                lL1FirstFollow.Add(new LL1FirstFollow(nonTerminal, _FirstByRuleOfNonTerminal, _First[nonTerminal], _Follow[nonTerminal], _Symbols));
            }

            return lL1FirstFollow;
        }

        //Source: https://github.com/PranayT17/Finding-FIRST-and-FOLLOW-of-given-grammar
        //Parsing Techniques - A Practical Guide 1st edition - page 168 - 8.2.1.1
        //Parsing Techniques - A Practical Guide 1st edition - page 171 - 8.2.2.1
        //Simon_Nesvera_-_Programovaci_jazyky_cviceni._2002_CVUT_FEL - page 30-31
        private void First(string rule)
        {
            var ruleSplitted = rule.Split("->");
            
            var nonTerminal = ruleSplitted[0].Trim();
            
            var productionSplitted = ruleSplitted[1].Trim().Split(" ");

            var symbol = productionSplitted[0].Trim();

            var remainingProduction = string.Join(" ", productionSplitted.Skip(1)).Trim();

            var first = FirstOfSymbol(symbol, remainingProduction);
            
            _FirstByRule[rule].UnionWith(first);
            
            _First[nonTerminal].UnionWith(first);
        }

        private HashSet<string> FirstOfSymbol(string symbol, string remainingProduction)
        {
            HashSet<string> first = new HashSet<string>();

            if (_Symbols._Terminals.Contains(symbol))
            {
                first.Add(symbol);
            }
            else if (_Symbols._NonTerminals.Contains(symbol))
            {
                foreach (var sym in _First[symbol])
                {
                    if (sym == LL1InputGrammar._EMPTY_EXPANSION)
                    {
                        var productionSplitted = remainingProduction.Split(" ");

                        var nextSymbol = productionSplitted[0].Trim();

                        var nextRemainingProduction = string.Join(" ", productionSplitted.Skip(1)).Trim();

                        first.UnionWith(FirstOfSymbol(nextSymbol, nextRemainingProduction));
                    }
                    else
                    {
                        first.Add(sym);
                    }
                }
            }
            else if ((symbol == LL1InputGrammar._EMPTY_EXPANSION) || (symbol == LL1InputGrammar._EMPTY_STRING))
            {
                first.Add(LL1InputGrammar._EMPTY_EXPANSION);
            }
            else
                throw new Exception("Symbol wan not eighter terminal or non terminal!");

            return first;
        }

        private void Follow(string rule)
        {
            var ruleSplitted = rule.Split("->");
            
            var nonTerminal = ruleSplitted[0].Trim();

            var productionSplitted = ruleSplitted[1].Trim().Split(" ");

            for (int i = 0; i < productionSplitted.Count(); i++)
            {
                var symbol = productionSplitted[i];

                var remainingProduction = productionSplitted.Count() == i ? string.Empty: string.Join(" ", productionSplitted.Skip(i + 1)).Trim();

                if (_Symbols._NonTerminals.Contains(symbol))
                {
                    _Follow[symbol].UnionWith(FollowOfSymbol(nonTerminal, remainingProduction));
                }
            }
        }

        private HashSet<string> FollowOfSymbol(string nonTerminal, string remainingProduction)
        {
            HashSet<string> follow = new HashSet<string>();

            string nextSymbol;

            if (remainingProduction == LL1InputGrammar._EMPTY_STRING)
            {
                nextSymbol = LL1InputGrammar._EMPTY_STRING;
            }
            else
            {
                nextSymbol = remainingProduction.Split(" ")[0];
            }

            if (_Symbols._Terminals.Contains(nextSymbol))
            {
                follow.Add(nextSymbol);
            }
            else if (_Symbols._NonTerminals.Contains(nextSymbol))
            {
                foreach (var sym in _First[nextSymbol])
                {
                    if (sym == LL1InputGrammar._EMPTY_EXPANSION)
                    {
                        follow.UnionWith(FollowOfSymbol(nonTerminal, string.Join(" ", remainingProduction.Split(" ").Skip(1))));
                    }
                    else
                    {
                        follow.Add(sym);
                    }
                }
            }
            else if (nextSymbol == LL1InputGrammar._EMPTY_STRING)
            {
                follow.UnionWith(_Follow[nonTerminal]);
            }

            return follow;
        }
    }
}
