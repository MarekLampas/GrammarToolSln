using GrammarTool.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class LL1WordParsing
    {
        public string _Word { get; set; }

        public string _RemainingWord { get; set; }

        public string _ParsingQueue { get; set; }

        //public string _OutputWord { get; set; }

        public List<StackTable> _StackTable { get; set; }

        public List<string> _ParsingQueuedTree;

        public List<Token> _Tokens { get; set; }

        //public List<Token> _ParsingQueueTokens { get; set; }

        public LL1WordParsing(string word, List<Token> tokens)
        {
            _Word = word.Replace("\n", " ");

            _RemainingWord = word.Replace("\n", " ") + LL1InputGrammar._END_STRING;

            _ParsingQueue = LL1InputGrammar._STARTING_SYMBOL + " " + LL1InputGrammar._END_STRING;

            _StackTable = new List<StackTable>();
            _StackTable.Add(new StackTable("Inserted word:", _Word));
            _StackTable.Add(new StackTable("Remaining word:", _RemainingWord));
            _StackTable.Add(new StackTable("Parsing stack", _ParsingQueue));

            _ParsingQueuedTree = new List<string>();

            _ParsingQueuedTree.Add(LL1InputGrammar._STARTING_SYMBOL + "0");

            _Tokens = tokens;
        }

        public class StackTable
        {
            public string _Title { get; set; }

            public string _Value { get; set; }

            public StackTable(string title, string value)
            {
                _Title = title;

                _Value = value;
            }
        }

        public void Consume()
        {
            _RemainingWord = string.Join(" ", _RemainingWord.Split(" ").Skip(1)).Trim();
            _StackTable[1]._Value = _RemainingWord;

            _ParsingQueue = string.Join(" ", _ParsingQueue.Split(" ").Skip(1)).Trim();
            _StackTable[2]._Value = _ParsingQueue;

            _ParsingQueuedTree.RemoveAt(_ParsingQueuedTree.Count - 1);
        }

        public void Expand(string production)
        {
            var productionStripped = production.Split("->")[1].Trim();

            _ParsingQueue = productionStripped == LL1InputGrammar._EMPTY_EXPANSION ?  string.Join(" ", _ParsingQueue.Split(" ").Skip(1)) : productionStripped + " " + string.Join(" ", _ParsingQueue.Split(" ").Skip(1));
            _StackTable[2]._Value = _ParsingQueue;

            _ParsingQueuedTree.RemoveAt(_ParsingQueuedTree.Count - 1);
        }

        public string GetRemaningWordSymbol()
        {
            return _RemainingWord.Split(" ")[0].Trim();
        }

        public string GetParsingQueueSymbol()
        {
            return _ParsingQueue.Split(" ")[0].Trim();
        }

        public string GetParsingQueuedTreeSymbol()
        {
            return _ParsingQueuedTree.Last();
        }
    }
}
