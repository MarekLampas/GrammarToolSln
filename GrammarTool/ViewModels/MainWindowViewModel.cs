using GrammarTool.Models;
using GrammarTool.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Text;

namespace GrammarTool.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel(Database db)
        {
            Rules = new GrammarPanelViewModel(db.GetRules());

            Content = new LandingPageViewModel();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public GrammarPanelViewModel Rules { get; }

        public void CreateGrammar()
        {
            var vm = new GrammarPanelViewModel(Rules.Rules);

            vm.Add.Subscribe(model =>
            {
                Rules.Rules.Add(model);

                //Was able to add just one rule - ReactiveCommand Subscribe() worked just once
                //Content = new GrammarPanelViewModel(Rules.Rules);
                CreateGrammar();
            });

            Content = vm;
        }
    }
}
