using GrammarTool.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;

namespace GrammarTool.ViewModels
{
    class GrammarPanelViewModel : ViewModelBase
    {
        string rule;

        public GrammarPanelViewModel(IEnumerable<GrammarRule> items)
        {
            Rules = new ObservableCollection<GrammarRule>(items);

            var addEnabled = this.WhenAnyValue(
                x => x.NewRule,
                x => !string.IsNullOrWhiteSpace(x));

            Add = ReactiveCommand.Create(
                () => new GrammarRule(rule: NewRule),
                addEnabled);
        }

        public string NewRule
        {
            get => rule;
            set => this.RaiseAndSetIfChanged(ref rule, value);
        }

        public ObservableCollection<GrammarRule> Rules { get; }

        //Source: https://docs.avaloniaui.net/tutorials/todo-list-app/adding-new-items
        //https://www.reactiveui.net/docs/handbook/commands/
        public ReactiveCommand<Unit, GrammarRule> Add { get; }
    }
}
