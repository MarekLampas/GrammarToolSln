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
using Avalonia.Controls;
using GrammarTool.Views;

namespace GrammarTool.ViewModels
{
    class GrammarPanelViewModel : ViewModelBase
    {
        string rule;

        public string InputText { get; set; }

        public string InputTextTokenized { get; set; }

        public string ErrorRule { get; set; }

        public bool AddRuleAnabled { get; set; }

        public GrammarPanelViewModel(Symbols symbols, string inputText, string inputTextTokenized, IEnumerable<LL1GrammarRule> rules, IEnumerable<LL1FirstFollow> firstFollow, LL1ParsingTable? lL1ParsingTable, LL1WordParsing lL1WordParsing, LL1ParsingTree lL1ParsingTree, Progress progressNote, bool hasOutput, string errorRule = "", bool addRuleAnabled = true)
        {
            InputText = inputText;

            InputTextTokenized = inputTextTokenized;

            ErrorRule = errorRule;

            AddRuleAnabled = addRuleAnabled;

            Grammar = new LL1Grammar(symbols, rules, firstFollow, lL1ParsingTable, lL1WordParsing, lL1ParsingTree, progressNote, hasOutput);

            var addEnabled = this.WhenAnyValue(
                x => x.NewRule,
                x => !string.IsNullOrWhiteSpace(x));

            //var submitEnabled = this.WhenAnyValue(
            //    x => x.Rules,
            //    x => (x.Rules.Count > 0));

            Add = ReactiveCommand.Create(
                () => new LL1GrammarRule(rule: NewRule),
                addEnabled);

            Submit = ReactiveCommand.Create(
                () => new LL1InputGrammar(symbols: Grammar._Symbols,rules:  Grammar._LL1Rules.Where(x => !string.IsNullOrEmpty(x.Rule))));

            Step = ReactiveCommand.Create(
                () => new LL1WordParsing(word: InputTextTokenized, tokens: Grammar._Symbols._Tokens, Grammar._LL1WordParsing._OutputWord, hasOutput: Grammar._HasOutput)/*,
                stepEnabled*/);

            Save = ReactiveCommand.Create(
                async () => await SaveFileExample());

            Back = ReactiveCommand.Create(
                () => StepEnabled);
        }

        public string NewRule
        {
            get => rule;
            set { this.RaiseAndSetIfChanged(ref rule, string.IsNullOrEmpty(value) ? rule : value); /*Grammar.GetSuggestions(rule); TODO: try to add AtoCompleteBox to Rules*/}
        }

        public LL1Grammar Grammar { get; }

        public bool PaneOpen
        {
            get => Grammar._LL1FirstFollow.Count > 0;
        }

        public bool PaneOpenNoWordInserted
        {
            get => (Grammar._LL1FirstFollow.Count > 0) && (string.IsNullOrWhiteSpace(Grammar._LL1WordParsing._Word));
        }

        public bool PaneClosedNoWordInserted
        {
            get => (Grammar._LL1FirstFollow.Count == 0) && (string.IsNullOrWhiteSpace(Grammar._LL1WordParsing._Word));
        }

        public bool WordInserted
        {
            get => !string.IsNullOrWhiteSpace(Grammar._LL1WordParsing._Word);
        }

        public bool StepEnabled
        {
            get => PaneOpenNoWordInserted || WordInserted;
        }

        private async Task<string> SaveFileExample()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Title = "Save example";
            saveFileDialog.Filters.Add(new FileDialogFilter() { Name = "Xml", Extensions = { "xml" } });
            saveFileDialog.InitialFileName = "Example.xml";
            string? outPathString = await saveFileDialog.ShowAsync(MainWindowView.Instance);
            return outPathString == null ? string.Empty : outPathString;
        }

        //Source: https://docs.avaloniaui.net/tutorials/todo-list-app/adding-new-items
        //https://www.reactiveui.net/docs/handbook/commands/
        public ReactiveCommand<Unit, LL1GrammarRule> Add { get; }

        public ReactiveCommand<Unit, LL1InputGrammar> Submit { get; }

        public ReactiveCommand<Unit, LL1WordParsing> Step { get; }

        public ReactiveCommand<Unit, Task<string>> Save { get; }

        public ReactiveCommand<Unit, bool> Back { get; }
    }
}
