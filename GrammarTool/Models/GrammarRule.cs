using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class GrammarRule
    {
        public string? Rule { get; set; }
        public bool IsValid { get; set; }

        public GrammarRule(string? rule, bool isValid = false)
        {
            Rule = rule;
        }
    }
}
