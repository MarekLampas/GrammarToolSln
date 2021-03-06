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

        public Scanner()
        {
        }

        public Scanner(string selected)
        {
            _scannerName = selected;

            if (selected == "C#")
                _tokenDefinitions = GetCSharpTokens();
            else if(selected == "None")
                _tokenDefinitions = GetEmptyTokens();
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

                        throw new Exception($"Invalid input found. Cannot tokenize value {invalidTokenMatch.Value}. It do not match any token definition.");
                    }
                }
            }

            symbols._Tokens.Add(new Token(TokenType.sequenceTerminator, string.Empty));

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
                    TokenType = TokenType.invalid,
                    Value = match.Value.Trim()
                };
            }

            throw new Exception("There was problem with input text. Failed to generate invalid token.");
        }
    }
}
