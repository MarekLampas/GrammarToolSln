using GrammarTool.Models;
using GrammarTool.Helpers;
using GrammarTool.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace GrammarTool.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel(Database db)
        {
            Grammar = new GrammarPanelViewModel(db.GetRules(), db.InicializeFirstFollow());

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
            var vm = new GrammarPanelViewModel(Grammar.Rules, Grammar.Grammar._LL1FirstFollow);

            vm.Add.Subscribe(model =>
            {
                Grammar.Rules.Add(model);

                //Was able to add just one rule - ReactiveCommand Subscribe() worked just once
                //Content = new GrammarPanelViewModel(Rules.Rules);
                CreateGrammar();
            });

            vm.Submit.Subscribe(model =>
            {
                var lL1ComputeFirstFollow = new LL1ComputeFirstFollow(model);

                Grammar.Grammar._LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(lL1ComputeFirstFollow.Compute(Grammar.Rules));

                //Content = new GrammarPanelViewModel(Grammar.Rules, Grammar.Grammar._LL1FirstFollow);
                CreateGrammar();
            });

            Content = vm;
        }
    }
}
