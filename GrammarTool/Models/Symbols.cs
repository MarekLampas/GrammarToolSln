using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GrammarTool.Helpers.ScannerTokenDefinitions;

namespace GrammarTool.Models
{
    public class Symbols
    {
        public List<string> _Terminals { get; set; }

        public List<Token> _Tokens { get; private set; }

        public List<string> _TokensUsed { get; set; }

        public List<string> _NonTerminals { get; private set; }

        public Symbols()
        {
            _Terminals = new List<string>();

            _Tokens = new List<Token>();

            _TokensUsed = new List<string>();

            _NonTerminals = new List<string>();
        }

        public void AddToken(TokenType tokenType, string value)
        {
            _Tokens.Add(new Token(tokenType, value));
        }

        public void AddTerminal(string terminal)
        {
            if (!_Terminals.Any(x => x.Equals(terminal)))
            {
                _Terminals.Add(terminal);
            }
        }

        public void AddNonTerminal(string nonTerminal)
        {
            if (!_NonTerminals.Any(x => x.Equals(nonTerminal)))
            {
                _NonTerminals.Add(nonTerminal);
            }
        }
    }

    public class Token
    {
        public TokenType _TokenType { get; set; }

        public string _Value { get; set; }

        public Token(TokenType tokenType, string value)
        {
            _TokenType = tokenType;

            _Value = value;
        }

        public Token(TokenType tokenType)
        {
            _TokenType = tokenType;

            _Value = string.Empty;
        }
    }
}
