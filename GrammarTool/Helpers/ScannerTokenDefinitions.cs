using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GrammarTool.Helpers
{
    //TODO: move this definitions to files on drive and load them when selecting scanner
    public class ScannerTokenDefinitions
    {
        public enum TokenType
        {
            [XmlEnum(Name = "Equals")]
            Equals,
            [XmlEnum(Name = "NotEquals")]
            NotEquals,
            [XmlEnum(Name = "Not")]
            Not,
            [XmlEnum(Name = "LessOrSameThan")]
            LessOrSameThan,
            [XmlEnum(Name = "MoreOrSameThan")]
            MoreOrSameThan,
            [XmlEnum(Name = "LessThan")]
            LessThan,
            [XmlEnum(Name = "MoreThan")]
            MoreThan,
            [XmlEnum(Name = "Assign")]
            Assign,
            [XmlEnum(Name = "Colon")]
            Colon,
            [XmlEnum(Name = "Semicolon")]
            Semicolon,
            [XmlEnum(Name = "Dot")]
            Dot,
            [XmlEnum(Name = "Comma")]
            Comma,
            [XmlEnum(Name = "RoundBracketOpen")]
            RoundBracketOpen,
            [XmlEnum(Name = "RoundBracketClose")]
            RoundBracketClose,
            [XmlEnum(Name = "CurlyBracketOpen")]
            CurlyBracketOpen,
            [XmlEnum(Name = "CurlyBracketClose")]
            CurlyBracketClose,
            [XmlEnum(Name = "SquareBracketOpen")]
            SquareBracketOpen,
            [XmlEnum(Name = "SquareBracketClose")]
            SquareBracketClose,
            [XmlEnum(Name = "Number")]
            Number,
            [XmlEnum(Name = "Increment")]
            Increment,
            [XmlEnum(Name = "Decrement")]
            Decrement,
            [XmlEnum(Name = "Plus")]
            Plus,
            [XmlEnum(Name = "Minus")]
            Minus,
            [XmlEnum(Name = "Multiply")]
            Multiply,
            [XmlEnum(Name = "Devide")]
            Devide,
            [XmlEnum(Name = "Variable")]
            Variable,
            [XmlEnum(Name = "DataType")]
            DataType,
            [XmlEnum(Name = "List")]
            List,
            [XmlEnum(Name = "Dictionary")]
            Dictionary,
            [XmlEnum(Name = "AccessModifier")]
            AccessModifier,
            [XmlEnum(Name = "Class")]
            Class,
            [XmlEnum(Name = "And")]
            And,
            [XmlEnum(Name = "Or")]
            Or,
            [XmlEnum(Name = "In")]
            In,
            [XmlEnum(Name = "If")]
            If,
            [XmlEnum(Name = "Elif")]
            Elif,
            [XmlEnum(Name = "Else")]
            Else,
            [XmlEnum(Name = "Foreach")]
            Foreach,
            [XmlEnum(Name = "For")]
            For,
            [XmlEnum(Name = "BoolValue")]
            BoolValue,
            [XmlEnum(Name = "Undefined")]
            Undefined,
            [XmlEnum(Name = "Switch")]
            Switch,
            [XmlEnum(Name = "Case")]
            Case,
            [XmlEnum(Name = "Break")]
            Break,
            [XmlEnum(Name = "Continue")]
            Continue,
            [XmlEnum(Name = "Print")]
            Print,
            [XmlEnum(Name = "StringValue")]
            StringValue,
            [XmlEnum(Name = "Identifier")]
            Identifier,
            [XmlEnum(Name = "SequenceTerminator")]
            SequenceTerminator,
            [XmlEnum(Name = "Invalid")]
            Invalid
        }

        public class TokenDefinition
        {
            public string _regexPattern { get; set; }
            [XmlIgnore]
            public Regex _regex { get; private set; }
            public TokenType _returnsToken { get; set; }
            public bool _isChecked { get; set; }

            public TokenDefinition()
            {
            }

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

            public void CreateRegex()
            {
                _regex = new Regex(_regexPattern, RegexOptions.IgnoreCase);
            }
        }

        public class TokenMatch
        {
            public bool IsMatch { get; set; }
            public TokenType TokenType { get; set; }
            public string Value { get; set; }
            public string RemainingText { get; set; }
        }

        public static List<TokenDefinition> GetEmptyTokens()
        {
            List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>();

            //If more rules takes same string, first found is used. So pay attance to order!
            foreach (var token in (TokenType[])Enum.GetValues(typeof(TokenType)))
            {
                _tokenDefinitions.Add(new TokenDefinition(token, string.Empty));
            }

            return _tokenDefinitions;
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
            _tokenDefinitions.Add(new TokenDefinition(TokenType.Number, "^\\d+\\.\\d+|^\\d+"));
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
            _tokenDefinitions.Add(new TokenDefinition(TokenType.BoolValue, "^true|^false"));
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
            _tokenDefinitions.Add(new TokenDefinition(TokenType.BoolValue, "^True|^False"));
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
