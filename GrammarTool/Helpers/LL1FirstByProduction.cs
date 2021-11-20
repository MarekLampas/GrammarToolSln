using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Helpers
{
    public class LL1FirstByProduction
    {
        public string _Production { get; set; }

        public string _Symbols { get; set; }

        public LL1FirstByProduction(string production, string symbols)
        {
            _Production = production;

            _Symbols = symbols;
        }
    }
}
