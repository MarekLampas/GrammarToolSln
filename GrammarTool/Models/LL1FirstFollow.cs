using GrammarTool.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class LL1FirstFollow
    {
        public string _NonTerminal { get; set; }

        public Dictionary<string, HashSet<string>> _FirstSetByProduction;

        public HashSet<string> _FirstSet;

        public string _FirstSetString { get; set; }

        public HashSet<string> _FollowSet;

        public string _FollowSetString { get; set; }

        public readonly Symbols _Symbols;

        public Dictionary<string, HashSet<string>> _TerminalToProduction;

        public LL1FirstFollow(string nonTerminal, Dictionary<string, HashSet<string>> firstSetByProduction, HashSet<string> firstSet, HashSet<string> followSet, Symbols symbols)
        {
            _NonTerminal = nonTerminal;

            _FirstSetByProduction = firstSetByProduction;

            _FirstSet = firstSet;

            _FirstSetString = string.Join(", ", _FirstSet);

            _FollowSet = followSet;

            _FollowSetString = string.Join(", ", _FollowSet);

            _Symbols = symbols;

            _TerminalToProduction = ComputeTerminalToProduction();


            File.AppendAllLines("first_follow.txt", new string[] {$"{_NonTerminal} First: ({string.Join(", ", _FirstSet)}) Follow: ({string.Join(", ", _FollowSet)})"});

            var hasFF = "Has FIRST-FIRST collision";
            var hasNotFF = "Doesn't have FIRST-FIRST collision";
            var ff = HasCollisionFirstFirst() ? hasFF : hasNotFF;
            
            File.AppendAllLines("first_follow.txt", new string[] { $"{ff}" });

            var hasFFol = "Has FIRST-FOLLOW collision";
            var hasNotFFol = "Doesn't have FIRST-FOLLOW collision";
            var ffol = HasCollisionFirstFollow() ? hasFFol : hasNotFFol;

            File.AppendAllLines("first_follow.txt", new string[] { $"{ffol}" });
        }

        private Dictionary<string, HashSet<string>> ComputeTerminalToProduction()
        {
            Dictionary<string, HashSet<string>> terminalToProduction = new Dictionary<string, HashSet<string>>();

            foreach (var terminal in _Symbols._TokensUsed)
            {
                terminalToProduction.Add(terminal, new HashSet<string>());
            }

            if (!terminalToProduction.ContainsKey(LL1InputGrammar._EMPTY_EXPANSION))
            {
                terminalToProduction.Add(LL1InputGrammar._EMPTY_EXPANSION, new HashSet<string>());
            }

            foreach (var productionSet in _FirstSetByProduction)
            {
                foreach (var symbolFirst in productionSet.Value)
                {
                    if (symbolFirst == LL1InputGrammar._EMPTY_EXPANSION)
                    {
                        foreach (var symbolFollow in _FollowSet)
                        {
                            if (symbolFollow == LL1InputGrammar._END_STRING)
                            {
                                if (!terminalToProduction.ContainsKey(symbolFirst))
                                    terminalToProduction.Add(symbolFirst, new HashSet<string>());

                                terminalToProduction[symbolFirst].Add(productionSet.Key);
                            }
                            else
                            {
                                if (!terminalToProduction.ContainsKey(symbolFollow))
                                    terminalToProduction.Add(symbolFollow, new HashSet<string>());

                                terminalToProduction[symbolFollow].Add(productionSet.Key);
                            }
                        }
                    }
                    else
                    {
                        if (!terminalToProduction.ContainsKey(symbolFirst))
                            terminalToProduction.Add(symbolFirst, new HashSet<string>());

                        terminalToProduction[symbolFirst].Add(productionSet.Key);
                    }
                }
            }

            return terminalToProduction;
        }

        public bool HasCollisionFirstFirst()
        {
            var keys = _FirstSetByProduction.Keys.ToArray();
            
            for (int i = 0; i < _FirstSetByProduction.Count; i++)
            {
                for(int j = i + 1; j < _FirstSetByProduction.Count; j++)
                {
                    if (_FirstSetByProduction[keys[i]].Overlaps(_FirstSetByProduction[keys[j]]))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool HasCollisionFirstFollow()
        {
            return _FirstSet.Overlaps(_FollowSet);
        }
    }
}
