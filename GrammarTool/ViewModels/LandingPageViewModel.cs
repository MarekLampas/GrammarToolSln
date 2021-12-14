using GrammarTool.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace GrammarTool.ViewModels
{
    public class LandingPageViewModel : ViewModelBase
    {
        public List<string> _ScannersAvailable { get; set; }

        public int _SelectedIndex { get; set; }

        public Scanner _selectedItem;

        public Scanner _SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public LandingPageViewModel()
        {
            //TODO:instead of this list load all scanners saved on drive - json or xml with node Name and list node tokens definitions saved under uniqeue filename
            _ScannersAvailable = new List<string>();
            _ScannersAvailable.Add("C#");
            _ScannersAvailable.Add("Python");

            //Source: https://stackoverflow.com/questions/59937058/c-wpf-avalonia-on-button-click-change-text - RaiseAndSetIfChanged() finaly made gui to refresh
            //dead end: https://stackoverflow.com/questions/9186979/how-to-use-selectedindexchanged-event-of-combobox
            //before I tried to create scanner instace after selecting availeble scanner from string list binded to combobox based on selectedIndex
            _Scanner = new List<Scanner>();
            foreach(var scanner in _ScannersAvailable)
            {
                _Scanner.Add(new Scanner(scanner));
            }

            _SelectedItem = _Scanner.First();
        }

        public List<Scanner> _Scanner { get; }
    }
}
