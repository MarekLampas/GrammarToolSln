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
        public IEnumerable<LL1GrammarRule> GetRules() => new List<LL1GrammarRule>();
        //{
        //    new LL1GrammarRule(rule: "S->A", isValid: true ),
        //    new LL1GrammarRule(rule: "A->aB", isValid: true ),
        //    new LL1GrammarRule(rule: "B->b", isValid: true )
        //};

        public IEnumerable<LL1FirstFollow> InicializeFirstFollow() => new List<LL1FirstFollow>();
    }
}
