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

        public MainWindowViewModel(Database db)
        {
            LandingPage = new LandingPageViewModel(string.Empty);

            LandingPage.OpenFileDialogCommand.Subscribe(async model =>
            {
                string path = await model;

                if (path != string.Empty) //else => dialog was canceled
                {
                    example = Deserialize(path);

                    LandingPage = new LandingPageViewModel(example.InputText == null ? string.Empty : example.InputText, example.SelectedIndex == null ? 0 : (int)example.SelectedIndex, example.IsChecked);

                    Content = LandingPage;
                }
            });

            Grammar = new GrammarPanelViewModel(new Symbols(), string.Empty, string.Empty, db.GetRules(), db.InicializeFirstFollow(), null, new LL1WordParsing(string.Empty, new List<Token>()), new LL1ParsingTree(), string.Empty);

            Content = LandingPage;
        }

        public ViewModelBase Content
        {
            get => content;
            private set => this.RaiseAndSetIfChanged(ref content, value);
        }

        public LandingPageViewModel LandingPage { get; set; }

        public GrammarPanelViewModel Grammar { get; }

        public void CreateGrammar()
        {
            if (Grammar.Grammar._Symbols._Terminals.Count == 0)
            {
                var symbols = LandingPage._SelectedItem._tokenDefinitions.Where(x => x._isChecked).Select(x => x._returnsToken.ToString()).ToList();

                symbols.Add(LL1InputGrammar._EMPTY_EXPANSION);

                Grammar.Grammar._Symbols._Terminals = symbols;

                LandingPage._InputTextTokenized = LandingPage._selectedItem.Tokenize(Grammar.Grammar._Symbols, LandingPage._inputText);

                Grammar.Grammar._Symbols._TokensUsed = Grammar.Grammar._Symbols._Tokens.Select(x => x._TokenType.ToString()).Distinct().OrderBy(x => x).ToList();

                if(example != null)
                {
                    if (example.Rules != null)
                    {
                        Grammar.Grammar._LL1Rules = new ObservableCollection<LL1GrammarRule>(example.Rules);
                    }
                }
            }

            //TODO: parsing table will not be updated if rules get changed!
            var vm = new GrammarPanelViewModel(Grammar.Grammar._Symbols, LandingPage._inputText, LandingPage._InputTextTokenized, Grammar.Grammar._LL1Rules, Grammar.Grammar._LL1FirstFollow, Grammar.Grammar._LL1ParsingTable, Grammar.Grammar._LL1WordParsing, Grammar.Grammar._LL1ParsingTree, Grammar.Grammar._ProgressNote);

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
                var lL1ComputeFirstFollow = new LL1ComputeFirstFollow(Grammar.Grammar._Symbols, model);

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

            vm.Save.Subscribe(async model =>
            {
                string path = await model;

                if (path != string.Empty) //else => dialog was canceled
                {
                    Example example = new Example(LandingPage._SelectedIndex, LandingPage._SelectedItem._tokenDefinitions.Select(x => x._isChecked).ToArray(), LandingPage._InputText, Grammar.Grammar._LL1Rules.ToArray());

                    var serialed = Serialize(example);

                    serialed = Regex.Replace(serialed, " encoding=\".+\"", "", RegexOptions.IgnoreCase);

                    File.WriteAllText(path, serialed);
                }
            });

            GrammarPanelView.SetParsingTableForView(vm.Grammar._LL1ParsingTable._LL1TerminalToProductions);

            Content = vm;
        }

        private string Serialize(Object o)
        {
            if (o == null)
            {
                return string.Empty;
            }
            try
            {
                using (var writer = new StringWriter())
                {
                    new XmlSerializer(o.GetType()).Serialize(writer, o);
                    return writer.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        private Example Deserialize(string path)
        {
            XmlSerializer ser = new XmlSerializer(typeof(Example));
            Example example;
            using (XmlReader reader = XmlReader.Create(path))
            {
                example = (Example)ser.Deserialize(reader);
            }

            return example;
        }
    }
}
