using Avalonia.Controls;
using GrammarTool.Models;
using GrammarTool.Helpers;
using GrammarTool.Views;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using static GrammarTool.Helpers.ScannerTokenDefinitions;

namespace GrammarTool.ViewModels
{
    class ScannerPanelViewModel : ViewModelBase
    {
        public int _SelectedIndex { get; set; }

        public Scanner _selectedItem;

        public Scanner _SelectedItem
        {
            get => _selectedItem;
            set => ScannerChanged(value);
        }

        public string _scannerNameInserted;

        public string _ScannerNameInserted
        {
            get => _scannerNameInserted;
            set => this.RaiseAndSetIfChanged(ref _scannerNameInserted, value);
        }

        public List<Scanner> _Scanner { get; set; }

        public Scanner _newScanner;

        public Scanner _NewScanner
        {
            get => _newScanner;
            set => this.RaiseAndSetIfChanged(ref _newScanner, value);
        }

        public TokenDefinition _selectedToken;

        public TokenDefinition _SelectedToken
        {
            get => _selectedToken;
            set => this.RaiseAndSetIfChanged(ref _selectedToken, value);
        }

        public ScannerPanelViewModel(List<Scanner> scanner, int selectedIndex = 0, bool withEmpty = false, string scannerName = "")
        {
            if (withEmpty)
            {
                _NewScanner = scanner[0];

                _Scanner = scanner;
            }
            else
            {
                _NewScanner = new Scanner("None");

                _Scanner = new List<Scanner>() { _NewScanner };

                _Scanner.AddRange(new List<Scanner>(scanner));
            }

            selectedIndex = selectedIndex < 0 ? 0 : selectedIndex;

            _SelectedIndex = selectedIndex;

            List<TokenDefinition> backup = new List<TokenDefinition>(_Scanner[selectedIndex]._tokenDefinitions);

            _Scanner[selectedIndex]._tokenDefinitions = new List<TokenDefinition>(_NewScanner._tokenDefinitions);

            _SelectedItem = _Scanner[selectedIndex];

            _selectedItem = _Scanner[selectedIndex];

            _ScannerNameInserted = scannerName;

            _scannerNameInserted = scannerName;

            _Scanner[selectedIndex]._tokenDefinitions = backup;

            var submitEnabled = this.WhenAnyValue(
                x => x._ScannerNameInserted,
                x => !string.IsNullOrWhiteSpace(x));

            MoveUp = ReactiveCommand.Create(
                () => ReorderScanner(true));

            MoveDown = ReactiveCommand.Create(
                () => ReorderScanner(false));

            Submit = ReactiveCommand.Create(
                () => CreateRegexForNewScanner(),
                submitEnabled);

            Cancel = ReactiveCommand.Create(
                () => _Scanner[0]);
        }

        private void ScannerChanged(Scanner value)
        {
            var newScanner = new Scanner("None");

            newScanner._tokenDefinitions = new List<TokenDefinition>(_NewScanner._tokenDefinitions);

            if (value == null)
            {
                foreach (var token in newScanner._tokenDefinitions)
                {
                    token._regexPattern = "";
                }
            }
            else
            {
                foreach (var token in newScanner._tokenDefinitions)
                {
                    var index = newScanner._tokenDefinitions.IndexOf(token);
                    if (index < value._tokenDefinitions.Count)
                    {
                        if (!Object.ReferenceEquals(token._regexPattern, value._tokenDefinitions[newScanner._tokenDefinitions.IndexOf(token)]._regexPattern))
                        {
                            token._regexPattern = "";
                        }
                    }
                    else
                    {
                        token._regexPattern = "";
                    }
                }

                foreach (var token in value._tokenDefinitions)
                {
                    newScanner._tokenDefinitions.Where(x => x._returnsToken == token._returnsToken).First()._regexPattern = token._regexPattern;
                }
            }

            _NewScanner = newScanner;

            _newScanner = newScanner;

            _Scanner[0] = _NewScanner;

            this.RaiseAndSetIfChanged(ref _selectedItem, value == null ? _Scanner[0] : value);
        }

        private Scanner ReorderScanner(bool up)
        {
            var index = _NewScanner._tokenDefinitions.IndexOf(_SelectedToken);

            if (index > -1)
            {
                if (up)
                {
                    if (index > 0)
                    {
                        _NewScanner._tokenDefinitions.Insert(index - 1, _NewScanner._tokenDefinitions[index]);

                        _NewScanner._tokenDefinitions.RemoveAt(index + 1);
                    }
                }
                else
                {
                    if (index < _NewScanner._tokenDefinitions.Count - 1)
                    {
                        _NewScanner._tokenDefinitions.Insert(index + 2, _NewScanner._tokenDefinitions[index]);

                        _NewScanner._tokenDefinitions.RemoveAt(index);
                    }
                }
            }

            return _NewScanner;
        }

        private Scanner CreateRegexForNewScanner()
        {
            for (int i = _NewScanner._tokenDefinitions.Count - 1; i > -1; i--)
            {
                if (string.IsNullOrEmpty(_NewScanner._tokenDefinitions[i]._regexPattern))
                {
                    _NewScanner._tokenDefinitions.RemoveAt(i);
                    continue;
                }
                if (!_NewScanner._tokenDefinitions[i]._regexPattern.StartsWith("^"))
                {
                    _NewScanner._tokenDefinitions[i]._regexPattern = $"^{_NewScanner._tokenDefinitions[i]._regexPattern}";
                }
                _NewScanner._tokenDefinitions[i].CreateRegex();
            }

            _newScanner._scannerName = _ScannerNameInserted;

            return _newScanner;
        }

        public ReactiveCommand<Unit, Scanner> MoveUp { get; }

        public ReactiveCommand<Unit, Scanner> MoveDown { get; }

        public ReactiveCommand<Unit, Scanner> Submit { get; }

        public ReactiveCommand<Unit, Scanner> Cancel { get; }
    }
}
