using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrammarTool.Models;

namespace GrammarTool.Services
{
    public class Database
    {
        /// <summary>
        /// Loading grammar rules from disk
        /// Paramater for new instance of GrammarPanelViewModel()
        /// </summary>
        /// <returns></returns>
        public IEnumerable<GrammarRule> GetRules() => new[]
        {
            new GrammarRule(rule: "S -> A", isValid: true ),
            new GrammarRule(rule: "A -> aB", isValid: true ),
            new GrammarRule(rule: "B -> b", isValid: true )
        };
    }
}
