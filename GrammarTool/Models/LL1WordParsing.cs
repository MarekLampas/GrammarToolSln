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

        public List<string> _ParsingQueuedTree;

        public LL1WordParsing(string word)
        {
            _Word = word;

            _RemainingWord = word + " " + LL1InputGrammar._END_STRING;

            _ParsingQueue = LL1InputGrammar._STARTING_SYMBOL + " " + LL1InputGrammar._END_STRING;

            _ParsingQueuedTree = new List<string>();

            _ParsingQueuedTree.Add(LL1InputGrammar._STARTING_SYMBOL + "0");
        }

        public void Consume()
        {
            _RemainingWord = string.Join(" ",_RemainingWord.Split(" ").Skip(1));

            _ParsingQueue = string.Join(" ", _ParsingQueue.Split(" ").Skip(1));

            _ParsingQueuedTree.RemoveAt(_ParsingQueuedTree.Count - 1);
        }

        public void Expand(string production)
        {
            var productionStripped = production.Split("->")[1].Trim();

            _ParsingQueue = productionStripped == LL1InputGrammar._EMPTY_EXPANSION ?  string.Join(" ", _ParsingQueue.Split(" ").Skip(1)) : productionStripped + " " + string.Join(" ", _ParsingQueue.Split(" ").Skip(1));

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
