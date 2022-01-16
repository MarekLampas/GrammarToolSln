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

        public string _OutputWord { get; set; }

        public List<StackTable> _StackTable { get; set; }

        public List<string> _ParsingQueuedTree;

        public List<Token> _Tokens { get; set; }

        public List<Token> _TokenStack { get; set; }

        public LL1WordParsing(string word, List<Token> tokens, string outputWord, bool hasOutput)
        {
            _Word = word.Replace("\n", " ");

            _RemainingWord = word.Replace("\n", " ") + LL1InputGrammar._END_STRING;

            _ParsingQueue = LL1InputGrammar._STARTING_SYMBOL + " " + LL1InputGrammar._END_STRING;

            _OutputWord = outputWord;

            _StackTable = new List<StackTable>();
            _StackTable.Add(new StackTable("Inserted word:", _Word));
            _StackTable.Add(new StackTable("Remaining word:", _RemainingWord));
            _StackTable.Add(new StackTable("Parsing stack", _ParsingQueue));
            if (hasOutput)
            {
                _StackTable.Add(new StackTable("Output word:", _OutputWord));
            }

            _ParsingQueuedTree = new List<string>();

            _ParsingQueuedTree.Add(LL1InputGrammar._STARTING_SYMBOL + "0");

            _Tokens = new List<Token>(tokens);

            _TokenStack = new List<Token>();
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

            var outputSymbols = productionStripped.Split(' ').Where(x => x.StartsWith("[") && x.EndsWith("]"));

            if (outputSymbols.Count() > 1)
            {
                throw new Exception("Rule can't contain more then one output symbol.");
            }
            else if (outputSymbols.Count() == 1)
            {
                var outputSymbol = outputSymbols.First();

                int count = _Tokens.Count();

                for (int i = 0; i < count; i++)
                {
                    if (_Tokens[i]._TokenType.ToString() == outputSymbol.Substring(1, outputSymbol.Length - 2))
                    {
                        _TokenStack.Insert(0, _Tokens[i]);

                        _Tokens.RemoveAt(i);

                        break;
                    }
                }

                if(_Tokens.Count() == count)
                {
                    throw new Exception("Output not found!");
                }
            }
        }

        public void AddOutput()
        {
            _ParsingQueue = string.Join(" ", _ParsingQueue.Split(" ").Skip(1)).Trim();
            _StackTable[2]._Value = _ParsingQueue;

            _OutputWord += $" {_TokenStack[0]._Value}";
            _OutputWord.Trim();
            _StackTable[3]._Value = _OutputWord;

            _TokenStack.RemoveAt(0);
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
