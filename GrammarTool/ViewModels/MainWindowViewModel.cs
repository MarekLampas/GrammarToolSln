using GrammarTool.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;

namespace GrammarTool.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        public MainWindowViewModel()
        {
            Content = new LandingPageViewModel();
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public void CreateGrammar()
        {
            Content = new GrammarPanelViewModel(new GrammarRule[] { new GrammarRule("A->AB") });
        }
    }
}
