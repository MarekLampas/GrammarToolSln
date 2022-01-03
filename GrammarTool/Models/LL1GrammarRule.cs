using AvaloniaGraphControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    [Serializable]
    public class LL1GrammarRule
    {
        public string? Rule { get; set; }
        public string? Output { get; set; }

        public LL1GrammarRule()
        {
        }

        public LL1GrammarRule(string? rule, bool isValid = false)
        {
            Rule = rule;
        }
    }
}
