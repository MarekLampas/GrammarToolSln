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

        public LL1WordParsing(string word)
        {
            _Word = word;

            _RemainingWord = word + LL1InputGrammar._END_STRING;

            _ParsingQueue = LL1InputGrammar._STARTING_SYMBOL + LL1InputGrammar._END_STRING;
        }

        public void Consume()
        {
            _RemainingWord = _RemainingWord.Substring(1);

            _ParsingQueue = _ParsingQueue.Substring(1);
        }

        public void Expand(string production)
        {
            var productionStripped = production.Split("->")[1];
            _ParsingQueue = productionStripped == LL1InputGrammar._EMPTY_EXPANSION ? _ParsingQueue.Substring(1) : productionStripped + _ParsingQueue.Substring(1);
        }

        public string GetRemaningWordSymbol()
        {
            return _RemainingWord.Substring(0, 1);
        }

        public string GetParsingQueueSymbol()
        {
            return _ParsingQueue.Substring(0, 1);
        }
    }
}
