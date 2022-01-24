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
            equals,
            [XmlEnum(Name = "NotEquals")]
            notEquals,
            [XmlEnum(Name = "Not")]
            not,
            [XmlEnum(Name = "LessOrSameThan")]
            lessOrSameThan,
            [XmlEnum(Name = "MoreOrSameThan")]
            moreOrSameThan,
            [XmlEnum(Name = "LessThan")]
            lessThan,
            [XmlEnum(Name = "MoreThan")]
            moreThan,
            [XmlEnum(Name = "Assign")]
            assign,
            [XmlEnum(Name = "Colon")]
            colon,
            [XmlEnum(Name = "Semicolon")]
            semicolon,
            [XmlEnum(Name = "Dot")]
            dot,
            [XmlEnum(Name = "Comma")]
            comma,
            [XmlEnum(Name = "RoundBracketOpen")]
            roundBracketOpen,
            [XmlEnum(Name = "RoundBracketClose")]
            roundBracketClose,
            [XmlEnum(Name = "CurlyBracketOpen")]
            curlyBracketOpen,
            [XmlEnum(Name = "CurlyBracketClose")]
            curlyBracketClose,
            [XmlEnum(Name = "SquareBracketOpen")]
            squareBracketOpen,
            [XmlEnum(Name = "SquareBracketClose")]
            squareBracketClose,
            [XmlEnum(Name = "Number")]
            number,
            [XmlEnum(Name = "Increment")]
            increment,
            [XmlEnum(Name = "Decrement")]
            decrement,
            [XmlEnum(Name = "Plus")]
            plus,
            [XmlEnum(Name = "Minus")]
            minus,
            [XmlEnum(Name = "Multiply")]
            multiply,
            [XmlEnum(Name = "Devide")]
            devide,
            [XmlEnum(Name = "Variable")]
            variable,
            [XmlEnum(Name = "DataType")]
            dataType,
            [XmlEnum(Name = "List")]
            list,
            [XmlEnum(Name = "Dictionary")]
            dictionary,
            [XmlEnum(Name = "AccessModifier")]
            accessModifier,
            [XmlEnum(Name = "Class")]
            classIdentifier,
            [XmlEnum(Name = "And")]
            and,
            [XmlEnum(Name = "Or")]
            or,
            [XmlEnum(Name = "In")]
            inside,
            [XmlEnum(Name = "If")]
            condition,
            [XmlEnum(Name = "Elif")]
            elseCondition,
            [XmlEnum(Name = "Else")]
            defaultCondition,
            [XmlEnum(Name = "Foreach")]
            foreachLoop,
            [XmlEnum(Name = "For")]
            forLoop,
            [XmlEnum(Name = "BoolValue")]
            boolValue,
            [XmlEnum(Name = "Undefined")]
            undefined,
            [XmlEnum(Name = "Switch")]
            switchCondition,
            [XmlEnum(Name = "Case")]
            caseCondition,
            [XmlEnum(Name = "Break")]
            breakLoop,
            [XmlEnum(Name = "Continue")]
            continueLoop,
            [XmlEnum(Name = "Print")]
            print,
            [XmlEnum(Name = "StringValue")]
            stringValue,
            [XmlEnum(Name = "Identifier")]
            identifier,
            [XmlEnum(Name = "SequenceTerminator")]
            sequenceTerminator,
            [XmlEnum(Name = "Invalid")]
            invalid
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
            _tokenDefinitions.Add(new TokenDefinition(TokenType.equals, "^=="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.notEquals, "^!="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.not, "^!"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.lessOrSameThan, "^<="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.moreOrSameThan, "^>="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.lessThan, "^<"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.moreThan, "^>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.assign, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.colon, "^:"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.semicolon, "^;"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.dot, "^\\."));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.roundBracketOpen, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.roundBracketClose, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.curlyBracketOpen, "^{"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.curlyBracketClose, "^}"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.squareBracketOpen, "^\\["));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.squareBracketClose, "^\\]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.number, "^\\d+\\.\\d+|^\\d+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.increment, "^\\+\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.decrement, "^--"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.plus, "^\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.minus, "^-"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.multiply, "^\\*"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.devide, "^/"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.variable, "^var"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.dataType, "^bool|^int|^float|^double|^string"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.list, "^List"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.dictionary, "^Dictionary"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.accessModifier, "^public|^private|^internal"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.classIdentifier, "^class"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.and, "^&&"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.or, "^\\|\\|"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.inside, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.condition, "^if"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.defaultCondition, "^else"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.foreachLoop, "^foreach"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.forLoop, "^for"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.boolValue, "^true|^false"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.undefined, "^null"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.switchCondition, "^switch"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.caseCondition, "^case"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.breakLoop, "^break"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.continueLoop, "^continue"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.print, "^Console.WriteLine|^Console.Write"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.stringValue, "^[\"'][^'\"]*['\"]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.identifier, "^[a-zA-Z][a-zA-Z0-9]*"));

            return _tokenDefinitions;
        }

        public static List<TokenDefinition> GetPythonTokens()
        {
            List<TokenDefinition> _tokenDefinitions = new List<TokenDefinition>();

            //If more rules takes same string, first found is used. So pay attance to order!
            _tokenDefinitions.Add(new TokenDefinition(TokenType.equals, "^=="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.notEquals, "^!="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.not, "^not"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.lessOrSameThan, "^<="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.moreOrSameThan, "^>="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.lessThan, "^<"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.moreThan, "^>"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.assign, "^="));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.colon, "^:"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.dot, "^\\."));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.comma, "^,"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.roundBracketOpen, "^\\("));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.roundBracketClose, "^\\)"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.curlyBracketOpen, "^{"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.curlyBracketClose, "^}"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.squareBracketOpen, "^\\["));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.squareBracketClose, "^\\]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.number, "^\\d+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.increment, "^\\+\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.decrement, "^--"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.plus, "^\\+"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.minus, "^-"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.multiply, "^\\*"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.devide, "^/"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.and, "^and"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.or, "^or"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.inside, "^in"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.condition, "^if"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.elseCondition, "^elif"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.defaultCondition, "^else"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.foreachLoop, "^foreach"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.forLoop, "^for"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.boolValue, "^True|^False"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.undefined, "^None"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.breakLoop, "^break"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.continueLoop, "^continue"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.print, "^print"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.stringValue, "^[\"'][^'\"]*['\"]"));
            _tokenDefinitions.Add(new TokenDefinition(TokenType.identifier, "^[a-zA-Z][a-zA-Z0-9]+"));

            return _tokenDefinitions;
        }
    }
}
