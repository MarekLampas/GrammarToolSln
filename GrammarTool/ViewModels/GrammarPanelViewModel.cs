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
using AvaloniaGraphControl;

namespace GrammarTool.ViewModels
{
    class GrammarPanelViewModel : ViewModelBase
    {
        string rule;

        string wordToParse;

        public GrammarPanelViewModel(IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow, LL1ParsingTable? lL1ParsingTable, LL1WordParsing lL1WordParsing, LL1ParsingTree lL1ParsingTree)
        {
            Grammar = new LL1Grammar(rules, firstFollow, lL1ParsingTable, lL1WordParsing, lL1ParsingTree);

            var addEnabled = this.WhenAnyValue(
                x => x.NewRule,
                x => !string.IsNullOrWhiteSpace(x));

            //var submitEnabled = this.WhenAnyValue(
            //    x => x.Rules,
            //    x => (x.Rules.Count > 0));

            var stepEnabled = this.WhenAnyValue(
                x => x.NewWordToParse,
                x => !string.IsNullOrWhiteSpace(x));

            Add = ReactiveCommand.Create(
                () => new LL1GrammarRule(rule: NewRule),
                addEnabled);

            Submit = ReactiveCommand.Create(
                () => new LL1InputGrammar(Grammar._LL1Rules));

            Step = ReactiveCommand.Create(
                () => new LL1WordParsing(word: NewWordToParse)/*,
                stepEnabled*/);
        }

        public string NewRule
        {
            get => rule;
            set => this.RaiseAndSetIfChanged(ref rule, value);
        }

        public string NewWordToParse
        {
            get => wordToParse;
            set => this.RaiseAndSetIfChanged(ref wordToParse, value);
        }

        public LL1Grammar Grammar { get; }

        public bool PaneOpen
        {
            get => Grammar._LL1FirstFollow.Count > 0;
        }

        public bool WordInserted
        {
            get => !string.IsNullOrWhiteSpace(Grammar._LL1WordParsing._Word);
        }

        //Source: https://docs.avaloniaui.net/tutorials/todo-list-app/adding-new-items
        //https://www.reactiveui.net/docs/handbook/commands/
        public ReactiveCommand<Unit, LL1GrammarRule> Add { get; }

        public ReactiveCommand<Unit, LL1InputGrammar> Submit { get; }

        public ReactiveCommand<Unit, LL1WordParsing> Step { get; }
    }
}
