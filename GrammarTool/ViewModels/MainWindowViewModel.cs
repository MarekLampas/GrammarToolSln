using GrammarTool.Models;
using GrammarTool.Helpers;
using GrammarTool.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Collections.ObjectModel;
using GrammarTool.Views;
using Avalonia.Media;

namespace GrammarTool.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel(Database db)
        {
            Grammar = new GrammarPanelViewModel(db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty), new LL1ParsingTree());

            Content = new LandingPageViewModel();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public GrammarPanelViewModel Grammar { get; }

        public void CreateGrammar()
        {
            //TODO: parsing table will not be updated if rules get changed!
            var vm = new GrammarPanelViewModel(Grammar.Grammar._LL1Rules, Grammar.Grammar._LL1FirstFollow, Grammar.Grammar._LL1ParsingTable, Grammar.Grammar._LL1WordParsing, Grammar.Grammar._LL1ParsingTree);

            vm.Add.Subscribe(model =>
            {
                Grammar.Grammar._LL1Rules.Add(model);

                //Was able to add just one rule - ReactiveCommand Subscribe() worked just once
                //Content = new GrammarPanelViewModel(Rules.Rules);
                CreateGrammar();
            });

            vm.Submit.Subscribe(model =>
            {
                var lL1ComputeFirstFollow = new LL1ComputeFirstFollow(model);

                Grammar.Grammar._LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(lL1ComputeFirstFollow.Compute(Grammar.Grammar._LL1Rules));

                //Content = new GrammarPanelViewModel(Grammar.Rules, Grammar.Grammar._LL1FirstFollow);
                CreateGrammar();
            });

            vm.Step.Subscribe(model =>
            {
                bool initialStep = false;
                //TODO: first step should be just adding S to production
                if (string.IsNullOrEmpty(Grammar.Grammar._LL1WordParsing._Word))
                {
                    Grammar.Grammar._LL1ParsingTable = ((GrammarPanelViewModel)Content).Grammar._LL1ParsingTable;
                    Grammar.Grammar._LL1WordParsing = model;

                    Grammar.Grammar._LL1ParsingTree.AddNode(LL1InputGrammar._STARTING_SYMBOL + "0", LL1InputGrammar._STARTING_SYMBOL, Brushes.LightGray);

                    initialStep = true;
                }

                if(!initialStep)
                    Grammar.Grammar.DoParseStep();

                CreateGrammar();
            });

            GrammarPanelView.SetParsingTableForView(vm.Grammar._LL1ParsingTable._LL1TerminalToProductions);

            Content = vm;
        }
    }
}
