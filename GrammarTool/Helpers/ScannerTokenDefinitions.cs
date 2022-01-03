using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GrammarTool.Helpers
{
    //TODO: move this definitions to files on drive and load them when selecting scanner
    public class ScannerTokenDefinitions
    {
        public enum TokenType
        {
            Undefined,
            Not,
            True,
            False,
            And,
            Or,
            Assign,
            Dot,
            Minus,
            Plus,
            Increment,
            Decrement,
            Multiply,
            Devide,
            Semicolon,
            Colon,
            Switch,
            Case,
            Break,
            Continue,
            LessOrSameThan,
            LessThan,
            MoreOrSameThan,
            MoreThan,
            Comma,
            Equals,
            NotEquals,
            Variable,
            DataType,
            List,
            Dictionary,
            AccessModifier,
            Class,
            In,
            NotIn,
            RoundBracketOpen,
            RoundBracketClose,
            CurlyBracketOpen,
            CurlyBracketClose,
            SquareBracketOpen,
            SquareBracketClose,
            Number,
            If,
            Elif,
            Else,
            Foreach,
            For,
            Print,
            StringValue,
            Identifier,
            SequenceTerminator,
            Invalid
        }

        public class TokenDefinition
        {
            public string _regexPattern { get; set; }
            public Regex _regex { get; }
            public TokenType _returnsToken { get; }
            public bool _isChecked { get; set; }

            public TokenDefinition(TokenType returnsToken, string regexPattern)
            {
                _regexPattern = regexPattern;
                _regex = new Regex(regexPattern, RegexOptions.IgnoreCase);
                _returnsToken = returnsToken;
                _isChecked = true;
            }

            public TokenMatch Match(string inputString)
            {
                var match = _regex.Match(inputString);
                if (match.Success)
                {
                    string remainingText = string.Empty;
                    if (match.Length != inputString.Length)
                        remainingText = inputString.Substring(match.Length);

                    return new TokenMatch()
                    {
                        IsMatch = true,
                        RemainingText = remainingText,
                        TokenType = _returnsToken,
                        Value = match.Value
                    };
                }
                else
                {
                    return new TokenMatch() { IsMatch = false };
                }

            }
        }

        public class TokenMatch
        {
            public bool IsMatch { get; set; }
            public TokenType TokenType { get; set; }
            public string Value { get; set; }
            public string RemainingText { get; set; }
        }

        public static List<TokenDefinition> GetCSharpTokens()
        {
            List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>();

            //If more rules takes same string, first found is used. So pay attance to order!
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "^=="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "^!="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Not, "^!"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LessOrSameThan, "^<="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreOrSameThan, "^>="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LessThan, "^<"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreThan, "^>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Assign, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Colon, "^:"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Semicolon, "^;"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Dot, "^\\."));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RoundBracketOpen, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RoundBracketClose, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CurlyBracketOpen, "^{"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CurlyBracketClose, "^}"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SquareBracketOpen, "^\\["));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SquareBracketClose, "^\\]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "^\\d+.\\d+|^\\d+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Increment, "^\\+\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Decrement, "^--"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Plus, "^\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Minus, "^-"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Multiply, "^\\*"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Devide, "^/"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Variable, "^var"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.DataType, "^bool|^int|^float|^double|^string"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.List, "^List"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Dictionary, "^Dictionary"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.AccessModifier, "^public|^private|^internal"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Class, "^class"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "^&&"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "^\\|\\|"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.In, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.If, "^if"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Else, "^else"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Foreach, "^foreach"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.For, "^for"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.True, "^true"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.False, "^false"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Undefined, "^null"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Switch, "^switch"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Case, "^case"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Break, "^break"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Continue, "^continue"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Print, "^Console.WriteLine|^Console.Write"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "^[\"'][^'\"]*['\"]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Identifier, "^[a-zA-Z][a-zA-Z0-9]*"));

            return _tokenDefinitions;
        }

        public static List<TokenDefinition> GetPythonTokens()
        {
            List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>();

            //If more rules takes same string, first found is used. So pay attance to order!
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Equals, "^=="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.NotEquals, "^!="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Not, "^not"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LessOrSameThan, "^<="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreOrSameThan, "^>="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.LessThan, "^<"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.MoreThan, "^>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Assign, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Colon, "^:"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Dot, "^\\."));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RoundBracketOpen, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.RoundBracketClose, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CurlyBracketOpen, "^{"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.CurlyBracketClose, "^}"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SquareBracketOpen, "^\\["));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.SquareBracketClose, "^\\]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "^\\d+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Increment, "^\\+\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Decrement, "^--"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Plus, "^\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Minus, "^-"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Multiply, "^\\*"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Devide, "^/"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.And, "^and"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Or, "^or"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.In, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.If, "^if"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Elif, "^elif"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Else, "^else"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Foreach, "^foreach"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.For, "^for"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.True, "^True"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.False, "^False"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Undefined, "^None"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Break, "^break"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Continue, "^continue"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Print, "^print"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.StringValue, "^[\"'][^'\"]*['\"]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Identifier, "^[a-zA-Z][a-zA-Z0-9]+"));

            return _tokenDefinitions;
        }
    }
}
