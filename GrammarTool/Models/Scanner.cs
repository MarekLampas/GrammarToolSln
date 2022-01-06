using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GrammarTool.Helpers.ScannerTokenDefinitions;

namespace GrammarTool.Models
{
    public class Scanner
    {
        public string _scannerName { get; set; }

        public List<TokenDefinition> _tokenDefinitions { get; set; }

        public Scanner(string selected)
        {
            _scannerName = selected;

            if (selected == "C#")
                _tokenDefinitions = GetCSharpTokens();
            else
                _tokenDefinitions = GetPythonTokens();
        }

        public string Tokenize(Symbols symbols, string lqlText)
        {
            string inputTextTokenized = string.Empty;

            string remainingText = lqlText;

            while (!string.IsNullOrWhiteSpace(remainingText))
            {
                var match = FindMatch(remainingText);
                if (match.IsMatch)
                {
                    symbols._Tokens.Add(new Token(match.TokenType, match.Value));
                    inputTextTokenized += match.TokenType.ToString() + " ";
                    remainingText = match.RemainingText;
                }
                else
                {
                    if (IsWhitespace(remainingText))
                    {
                        var nextCharacter = remainingText[0];
                        if (nextCharacter == '\n')
                            inputTextTokenized += nextCharacter;
                        remainingText = remainingText.Substring(1);
                    }
                    else
                    {
                        var invalidTokenMatch = CreateInvalidTokenMatch(remainingText);
                        symbols._Tokens.Add(new Token(invalidTokenMatch.TokenType, invalidTokenMatch.Value));
                        remainingText = invalidTokenMatch.RemainingText;

                        throw new Exception($"Found invalid token {invalidTokenMatch.Value}!");
                    }
                }
            }

            symbols._Tokens.Add(new Token(TokenType.SequenceTerminator, string.Empty));

            return inputTextTokenized;
        }

        private TokenMatch FindMatch(string lqlText)
        {
            foreach (var tokenDefinition in _tokenDefinitions)
            {
                var match = tokenDefinition.Match(lqlText);
                if (match.IsMatch)
                    return match;
            }

            return new TokenMatch() { IsMatch = false };
        }

        private bool IsWhitespace(string lqlText)
        {
            return Regex.IsMatch(lqlText, "^\\s+");
        }

        private TokenMatch CreateInvalidTokenMatch(string lqlText)
        {
            var match = Regex.Match(lqlText, "(^\\S+\\s)|^\\S+");
            if (match.Success)
            {
                return new TokenMatch()
                {
                    IsMatch = true,
                    RemainingText = lqlText.Substring(match.Length),
                    TokenType = TokenType.Invalid,
                    Value = match.Value.Trim()
                };
            }

            throw new Exception("Failed to generate invalid token");
        }
    }
}
