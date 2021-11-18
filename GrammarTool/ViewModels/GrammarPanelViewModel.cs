using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;
using GrammarTool.Helpers;

namespace GrammarTool.ViewModels
{
    class GrammarPanelViewModel : ViewModelBase
    {
        string rule;

        public GrammarPanelViewModel(IEnumerable<GrammarRule> items, IEnumerable<LL1FirstFollow> firstFollow)
        {
            Rules = new ObservableCollection<GrammarRule>(items);

            Grammar = new LL1Grammar(firstFollow);

            var addEnabled = this.WhenAnyValue(
                x => x.NewRule,
                x => !string.IsNullOrWhiteSpace(x));

            //var submitEnabled = this.WhenAnyValue(
            //    x => x.Rules,
            //    x => (x.Rules.Count > 0));

            Add = ReactiveCommand.Create(
                () => new GrammarRule(rule: NewRule),
                addEnabled);

            Submit = ReactiveCommand.Create(
                () => new LL1InputGrammar(Rules));
        }

        public string NewRule
        {
            get => rule;
            set => this.RaiseAndSetIfChanged(ref rule, value);
        }

        public ObservableCollection<GrammarRule> Rules { get; }

        public LL1Grammar Grammar { get; }

        //Source: https://docs.avaloniaui.net/tutorials/todo-list-app/adding-new-items
        //https://www.reactiveui.net/docs/handbook/commands/
        public ReactiveCommand<Unit, GrammarRule> Add { get; }

        public ReactiveCommand<Unit, LL1InputGrammar> Submit { get; }
    }
}
