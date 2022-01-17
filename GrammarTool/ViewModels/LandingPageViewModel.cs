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

namespace GrammarTool.ViewModels
{
    public class LandingPageViewModel : ViewModelBase
    {
        public readonly string _ScannerPath = @"Scanners";
        public readonly string _ScannerPathCustom = @"Scanners\Custom";

        public List<string> _ScannersBuildIn { get; set; }

        public int _SelectedIndex { get; set; }

        public Scanner _selectedItem;

        public Scanner _SelectedItem
        {
            get => _selectedItem;
            set => this.RaiseAndSetIfChanged(ref _selectedItem, value);
        }

        public string _inputText;

        public string _InputText
        {
            get => _inputText;
            set => this.RaiseAndSetIfChanged(ref _inputText, value);
        }

        public string _InputTextTokenized { get; set; }

        public LandingPageViewModel(string inputText, int selectedIndex = 0, bool[]? isChecked = null)
        {
            _InputText = inputText;
            _inputText = inputText;

            _SelectedIndex = selectedIndex;

            OpenFileDialogCommand = ReactiveCommand.Create(
                async () => await ChooseFileExample());

            //TODO:instead of this list load all scanners saved on drive - json or xml with node Name and list node tokens definitions saved under uniqeue filename
            _ScannersBuildIn = new List<string>();
            _ScannersBuildIn.Add("CSharp");
            _ScannersBuildIn.Add("Python");

            //Source: https://stackoverflow.com/questions/59937058/c-wpf-avalonia-on-button-click-change-text - RaiseAndSetIfChanged() finaly made gui to refresh
            //dead end: https://stackoverflow.com/questions/9186979/how-to-use-selectedindexchanged-event-of-combobox
            //before I tried to create scanner instace after selecting availeble scanner from string list binded to combobox based on selectedIndex
            _Scanner = new List<Scanner>();
            foreach (var scannerName in _ScannersBuildIn)
            {
                var scanner = XmlUtils.DeserializeScanner(Path.Combine(_ScannerPath, $"{scannerName}.xml"));
                foreach (var tokenDefinition in scanner._tokenDefinitions)
                {
                    tokenDefinition.CreateRegex();
                }
                _Scanner.Add(scanner);

                //File.WriteAllText(Path.Combine(_ScannerPath, scannerName + ".xml"), XmlUtils.Serialize(new Scanner(scannerName == "CSharp" ? "C#" : scannerName)));
            }

            var scannersCustom = Directory.GetFiles(_ScannerPathCustom, "*.xml", SearchOption.AllDirectories);

            foreach (var scannerName in scannersCustom)
            {
                var scanner = XmlUtils.DeserializeScanner(scannerName);
                foreach (var tokenDefinition in scanner._tokenDefinitions)
                {
                    tokenDefinition.CreateRegex();
                }
                _Scanner.Add(scanner);
            }

            _SelectedItem = _Scanner[selectedIndex];
            _selectedItem = _Scanner[selectedIndex];

            if (isChecked != null)
            {
                for (int i = 0; i < isChecked.Length; i++)
                {
                    _SelectedItem._tokenDefinitions[i]._isChecked = isChecked[i];
                    _selectedItem._tokenDefinitions[i]._isChecked = isChecked[i];
                }
            }
        }

        private async Task<string> ChooseFileExample()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Open example";
            openFileDialog.Filters.Add(new FileDialogFilter() { Name = "Xml", Extensions = { "xml" } });
            openFileDialog.AllowMultiple = true;
            string[]? outPathStrings = await openFileDialog.ShowAsync(MainWindowView.Instance);
            return outPathStrings == null ? string.Empty : outPathStrings[0];
        }

        public List<Scanner> _Scanner { get; }

        public ReactiveCommand<Unit, Task<string>> OpenFileDialogCommand { get; }
    }
}
