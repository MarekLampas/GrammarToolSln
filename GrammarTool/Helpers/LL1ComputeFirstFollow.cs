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

        public Dictionary<string, HashSet<string>> _First;

        Dictionary<string, HashSet<string>> _FirstByRule;

        public Dictionary<string, HashSet<string>> _Follow;

        Dictionary<string, List<string>> _NonTerminalToRules;

        LL1InputGrammar _LL1InputGrammar;

        public LL1ComputeFirstFollow(LL1InputGrammar lL1InputGrammar)
        {
            _First = new Dictionary<string, HashSet<string>>();

            _FirstByRule = new Dictionary<string, HashSet<string>>();

            _Follow = new Dictionary<string, HashSet<string>>();

            _NonTerminalToRules = new Dictionary<string, List<string>>();

            _LL1InputGrammar = lL1InputGrammar;
        }

        public IEnumerable<LL1FirstFollow> Compute(IEnumerable<GrammarRule> rules)
        {
            List<LL1FirstFollow> lL1FirstFollow = new List<LL1FirstFollow>();

            foreach (var nonTerminal in _LL1InputGrammar._NonTerminals)
            {
                _First.Add(nonTerminal, new HashSet<string>());
                _Follow.Add(nonTerminal, new HashSet<string>());
            }

            foreach (var rule in rules)
            {
                var individualRules = new List<string>() { rule.Rule };

                if (!_NonTerminalToRules.ContainsKey(rule.Rule.Substring(0, 1)))
                {
                    _NonTerminalToRules.Add(rule.Rule.Substring(0, 1), new List<string>());
                }

                if (rule.Rule.Contains('/'))
                {
                    var nonTerminalToProductions = rule.Rule.Split("->");
                    
                    individualRules = nonTerminalToProductions[1].Split('/').Select(x => $"{nonTerminalToProductions[0]}->{x}").ToList();
                }
                foreach (var individualRule in individualRules)
                {
                    _FirstByRule.Add(individualRule, new HashSet<string>());

                    _NonTerminalToRules[individualRule.Substring(0, 1)].Add(individualRule);
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
                    First(rule);
                    
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

                foreach (var nonTerminal in _LL1InputGrammar._NonTerminals)
                {
                    _followOld.Add(nonTerminal, _Follow[nonTerminal].Select(x => x).ToHashSet());
                }

                bool wasChanged = false;

                foreach (var rule in _FirstByRule.Keys)
                {
                    Follow(rule);
                }

                foreach (var nonTerminal in _Follow.Keys)
                {
                    if (!(_Follow[nonTerminal].SetEquals(_followOld[nonTerminal])))
                        wasChanged = true;
                }

                if (!wasChanged)
                    break;
            }

            foreach(var nonTerminal in _LL1InputGrammar._NonTerminals)
            {
                Dictionary<string, HashSet<string>> _FirstByRuleOfNonTerminal = new Dictionary<string, HashSet<string>>();

                foreach(var firstByRule in _FirstByRule)
                {
                    if (_NonTerminalToRules[nonTerminal].Contains(firstByRule.Key))
                    {
                        _FirstByRuleOfNonTerminal.Add(firstByRule.Key, firstByRule.Value);
                    }
                }

                lL1FirstFollow.Add(new LL1FirstFollow(nonTerminal, _FirstByRuleOfNonTerminal, _First[nonTerminal], _Follow[nonTerminal], _LL1InputGrammar));
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
            
            var nonTerminal = ruleSplitted[0];
            
            var production = ruleSplitted[1];

            var first = FirstOfSymbol(production.Substring(0, 1), production.Substring(1));
            
            _FirstByRule[rule].UnionWith(first);
            
            _First[nonTerminal].UnionWith(first);
        }

        private HashSet<string> FirstOfSymbol(string symbol, string remainingProduction)
        {
            HashSet<string> first = new HashSet<string>();

            if (_LL1InputGrammar._Terminals.Contains(symbol))
            {
                first.Add(symbol);
            }
            else if (_LL1InputGrammar._NonTerminals.Contains(symbol))
            {
                foreach (var sym in _First[symbol])
                {
                    if (sym == LL1InputGrammar._EMPTY_EXPANSION)
                    {
                        first.UnionWith(FirstOfSymbol(remainingProduction.Substring(0, 1), remainingProduction.Substring(1)));
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
            
            var nonTerminal = ruleSplitted[0];
            
            var production = ruleSplitted[1];

            for (int i = 0; i < production.Length; i++)
            {
                var symbol = production.Substring(i, 1);

                if (_LL1InputGrammar._NonTerminals.Contains(symbol))
                {
                    _Follow[symbol].UnionWith(FollowOfSymbol(nonTerminal, production.Substring(i + 1)));
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
                nextSymbol = remainingProduction.Substring(0, 1);
            }

            if (_LL1InputGrammar._Terminals.Contains(nextSymbol))
            {
                follow.Add(nextSymbol);
            }
            else if (_LL1InputGrammar._NonTerminals.Contains(nextSymbol))
            {
                foreach (var sym in _First[nextSymbol])
                {
                    if (sym == LL1InputGrammar._EMPTY_EXPANSION)
                    {
                        follow.UnionWith(FollowOfSymbol(nonTerminal, remainingProduction.Substring(1)));
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
