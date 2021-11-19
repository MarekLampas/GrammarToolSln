﻿using GrammarTool.Helpers;
using System;
using System.Collections.Generic;
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

        public LL1InputGrammar _LL1InputGrammar;

        public LL1FirstFollow(string nonTerminal, Dictionary<string, HashSet<string>> firstSetByProduction, HashSet<string> firstSet, HashSet<string> followSet, LL1InputGrammar inputLL1Grammar)
        {
            _NonTerminal = nonTerminal;

            _FirstSetByProduction = firstSetByProduction;

            _FirstSet = firstSet;

            _FirstSetString = string.Join(", ", _FirstSet);

            _FollowSet = followSet;

            _FollowSetString = string.Join(", ", _FollowSet);

            _LL1InputGrammar = inputLL1Grammar;

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