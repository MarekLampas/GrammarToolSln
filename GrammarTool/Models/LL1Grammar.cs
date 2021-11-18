using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.Models
{
    public class LL1Grammar
    {
        public ObservableCollection<LL1FirstFollow> _LL1FirstFollow { get; set; }

        public LL1Grammar(IEnumerable<LL1FirstFollow> firstFollow)
        {
            _LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(firstFollow);
        }
    }
}
