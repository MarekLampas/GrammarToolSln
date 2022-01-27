using GrammarTool.Models;
using GrammarTool.Helpers;
using GrammarTool.Services;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.ObjectModel;
using GrammarTool.Views;
using Avalonia.Media;
using System.Linq;
using System.IO;
using System.Xml.Serialization;
using System.Xml;
using System.Text.RegularExpressions;

namespace GrammarTool.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        ViewModelBase content;

        Example example;

        bool addRuleAnabled;

        Database db;

        public MainWindowViewModel(Database _db)
        {
            db = _db;

            LandingPage = new LandingPageViewModel(string.Empty);

            ScannerPage = new ScannerPanelViewModel(LandingPage._Scanner);

            LandingPage.OpenFileDialogCommand.Subscribe(async model =>
            {
                string path = await model;

                if (path != string.Empty) //else => dialog was canceled
                {
                    example = XmlUtils.DeserializeExample(path);

                    LandingPage = new LandingPageViewModel(example.InputText == null ? string.Empty : example.InputText, example.SelectedIndex == null ? 0 : (int)example.SelectedIndex, example.IsChecked);

                    OpenExample();
                }
            });

            Grammar = new GrammarPanelViewModel(new Symbols(), string.Empty, string.Empty, db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>(), string.Empty, false), new LL1ParsingTree(), new Progress(), new List<string>(), false);

            Content = LandingPage;
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public LandingPageViewModel LandingPage { get; set; }

        public ScannerPanelViewModel ScannerPage { get; set; }

        public GrammarPanelViewModel Grammar { get; set; }

        public void CreateGrammar()
        {
            if (Grammar.Grammar._Symbols._Terminals.Count == 0)
            {
                addRuleAnabled = true;

                var symbols = ((LandingPageViewModel)Content)._SelectedItem._tokenDefinitions.Where(x => x._isChecked).Select(x => x._returnsToken.ToString()).ToList();

                symbols.Add(LL1InputGrammar._EMPTY_EXPANSION);

                Grammar.Grammar._Symbols._Terminals = symbols;

                LandingPage._InputText = ((LandingPageViewModel)Content)._inputText;

                try
                {
                    LandingPage._InputTextTokenized = ((LandingPageViewModel)Content)._selectedItem.Tokenize(Grammar.Grammar._Symbols, ((LandingPageViewModel)Content)._inputText);
                }
                catch(Exception e)
                {
                    Grammar.ErrorRule = e.Message;

                    addRuleAnabled = false;
                }

                LandingPage._SelectedItem._tokenDefinitions = ((LandingPageViewModel)Content)._SelectedItem._tokenDefinitions;

                Grammar.Grammar._Symbols._TokensUsed = Grammar.Grammar._Symbols._Tokens.Select(x => x._TokenType.ToString()).Distinct().OrderBy(x => x).ToList();
            }

            if ((example != null) && (Content is LandingPageViewModel))
            {
                if ((example.Rules != null) && (example.InputText == ((LandingPageViewModel)Content)._InputText))
                {
                    Grammar.Grammar._LL1Rules = new ObservableCollection<LL1GrammarRule>(example.Rules);
                }
                else
                {
                    Grammar.Grammar._LL1Rules = new ObservableCollection<LL1GrammarRule>();
                }
            }

            //TODO: parsing table will not be updated if rules get changed!
            var vm = new GrammarPanelViewModel(Grammar.Grammar._Symbols, LandingPage._inputText, LandingPage._InputTextTokenized, Grammar.Grammar._LL1Rules, Grammar.Grammar._LL1FirstFollow, Grammar.Grammar._LL1ParsingTable, Grammar.Grammar._LL1WordParsing, Grammar.Grammar._LL1ParsingTree, Grammar.Grammar._ProgressNote, Grammar.Grammar._UsedRules, Grammar.Grammar._HasOutput, Grammar.ErrorRule, addRuleAnabled);

            vm.Add.Subscribe(model =>
            {
                //TODO: verify if all terminals are valid? All used are already known
                Grammar.Grammar._LL1Rules.Add(model);

                //Was able to add just one rule - ReactiveCommand Subscribe() worked just once
                //Content = new GrammarPanelViewModel(Rules.Rules);
                CreateGrammar();
            });

            vm.Submit.Subscribe(model =>
            {
                Grammar.Grammar._LL1Rules = new ObservableCollection<LL1GrammarRule>(Grammar.Grammar._LL1Rules.Where(x => !string.IsNullOrEmpty(x.Rule)));

                if (string.IsNullOrEmpty(model._Error))
                {
                    var lL1ComputeFirstFollow = new LL1ComputeFirstFollow(Grammar.Grammar._Symbols, model);

                    Grammar.Grammar._LL1FirstFollow = new ObservableCollection<LL1FirstFollow>(lL1ComputeFirstFollow.Compute(Grammar.Grammar._LL1Rules));

                    Grammar.Grammar._HasOutput = model._HasOutput;

                    //Content = new GrammarPanelViewModel(Grammar.Rules, Grammar.Grammar._LL1FirstFollow);
                    CreateGrammar();
                }
                else
                {
                    Grammar.ErrorRule = model._Error;

                    CreateGrammar();
                }
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

                    Grammar.Grammar._UsedRules = new List<string>();

                    initialStep = true;
                }

                if(!initialStep)
                    Grammar.Grammar.DoParseStep();

                CreateGrammar();
            });

            vm.Save.Subscribe(async model =>
            {
                string path = await model;

                if (path != string.Empty) //else => dialog was canceled
                {
                    Example example = new Example(LandingPage._SelectedIndex, LandingPage._SelectedItem._tokenDefinitions.Select(x => x._isChecked).ToArray(), LandingPage._InputText, Grammar.Grammar._LL1Rules.ToArray());

                    var serialed = XmlUtils.Serialize(example);

                    serialed = Regex.Replace(serialed, " encoding=\".+\"", "", RegexOptions.IgnoreCase);

                    File.WriteAllText(path, serialed);
                }
            });

            vm.Back.Subscribe(model =>
            {
                if (model)
                {
                    Grammar = new GrammarPanelViewModel(Grammar.Grammar._Symbols, LandingPage._inputText, LandingPage._InputTextTokenized, Grammar.Grammar._LL1Rules, db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>(), string.Empty, false), new LL1ParsingTree(), new Progress(), new List<string>(), false);

                    CreateGrammar();
                }
                else
                {
                    Grammar = new GrammarPanelViewModel(new Symbols(), string.Empty, string.Empty, db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>(), string.Empty, false), new LL1ParsingTree(), new Progress(), new List<string>(), false);

                    OpenExample();
                }
                return;
            });

            vm.Refresh.Subscribe(model =>
            {
                Grammar.Grammar._LL1Rules = new ObservableCollection<LL1GrammarRule>(Grammar.Grammar._LL1Rules.Where(x => !string.IsNullOrEmpty(x.Rule)));
                foreach(var rule in Grammar.Grammar._LL1Rules)
                {
                    rule.Rule = rule.Rule.Replace(LL1InputGrammar._EMPTY_EXPANSION_INSERT, LL1InputGrammar._EMPTY_EXPANSION);
                }

                CreateGrammar();
            });

            GrammarPanelView.SetParsingTableForView(vm.Grammar._LL1ParsingTable._LL1TerminalToProductions);

            Content = vm;
        }

        public void OpenExample()
        {
            var vm = new LandingPageViewModel(LandingPage._InputText, LandingPage._SelectedIndex, LandingPage._SelectedItem._tokenDefinitions.Select(x => x._isChecked).ToArray());

            vm.OpenFileDialogCommand.Subscribe(async model =>
            {
                string path = await model;

                if (path != string.Empty) //else => dialog was canceled
                {
                    example = XmlUtils.DeserializeExample(path);

                    LandingPage = new LandingPageViewModel(example.InputText == null ? string.Empty : example.InputText, example.SelectedIndex == null ? 0 : (int)example.SelectedIndex, example.IsChecked);

                    Grammar = new GrammarPanelViewModel(new Symbols(), string.Empty, string.Empty, db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>(), string.Empty, false), new LL1ParsingTree(), new Progress(), new List<string>(), false);

                    OpenExample();
                }
            });

            Content = vm;
        }

        public void CreateScanner()
        {
            var vm = new ScannerPanelViewModel(ScannerPage._Scanner, ScannerPage._SelectedIndex, true, ScannerPage._ScannerNameInserted);

            vm.MoveUp.Subscribe(model =>
            {
                ScannerPage._Scanner = ((ScannerPanelViewModel)Content)._Scanner;

                ScannerPage._SelectedIndex = ((ScannerPanelViewModel)Content)._SelectedIndex;

                ScannerPage._ScannerNameInserted = ((ScannerPanelViewModel)Content)._ScannerNameInserted;

                CreateScanner();
            });

            vm.MoveDown.Subscribe(model =>
            {
                ScannerPage._Scanner = ((ScannerPanelViewModel)Content)._Scanner;

                ScannerPage._SelectedIndex = ((ScannerPanelViewModel)Content)._SelectedIndex;

                ScannerPage._ScannerNameInserted = ((ScannerPanelViewModel)Content)._ScannerNameInserted;

                CreateScanner();
            });

            vm.Submit.Subscribe(model =>
            {
                var serialed = XmlUtils.Serialize(model);

                serialed = Regex.Replace(serialed, " encoding=\".+\"", "", RegexOptions.IgnoreCase);

                var scannerName = XmlUtils.RemoveSpecialCharacters(((ScannerPanelViewModel)Content)._ScannerNameInserted);

                if (!string.IsNullOrEmpty(scannerName))
                {
                    var count = Directory.GetFiles(LandingPage._ScannerPathCustom, $"{scannerName}*.xml").Length;

                    scannerName = count == 0 ? $"{scannerName}.xml" : $"{scannerName}_{count}.xml";
                }
                else
                {
                    var count = Directory.GetFiles(LandingPage._ScannerPathCustom, $"customScanner*.xml").Length;

                    scannerName = count == 0 ? $"customScanner.xml" : $"customScanner_{count}.xml";
                }

                File.WriteAllText(Path.Combine(LandingPage._ScannerPathCustom, scannerName), serialed);

                LandingPage = new LandingPageViewModel(string.Empty);

                OpenExample();
            });

            vm.Cancel.Subscribe(model =>
            {
                Grammar = new GrammarPanelViewModel(new Symbols(), string.Empty, string.Empty, db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>(), string.Empty, false), new LL1ParsingTree(), new Progress(), new List<string>(), false);

                OpenExample();
            });

            Content = vm;
        }
    }
}
