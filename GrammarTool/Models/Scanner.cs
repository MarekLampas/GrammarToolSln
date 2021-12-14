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
    }
}
