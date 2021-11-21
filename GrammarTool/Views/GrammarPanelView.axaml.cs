using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using System.Linq;

namespace GrammarTool.Views
{
    public partial class GrammarPanelView : UserControl
    {
        private static ObservableCollection<string[]> _ParsingTable;
        public GrammarPanelView()
        {
            InitializeComponent();

            //Source: https://stackoverflow.com/questions/65704754/how-to-bind-datagrid-items-with-observablecollectionsometype
            //only way to have table whith changing column count
            if (_ParsingTable.Count > 0)
            {
                var grid = this.Get<DataGrid>("ParsingTable");

                foreach (var idx in _ParsingTable[0].Select((value, index) => index))
                {
                    grid.Columns.Add(new DataGridTextColumn { Header = $"{_ParsingTable[0][idx]}", Binding = new Binding($"[{idx}]") });
                }

                grid.AutoGenerateColumns = false;
                grid.Items = new ObservableCollection<string[]>(_ParsingTable.Skip(1));
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public static void SetParsingTableForView(ObservableCollection<string[]> t)
        {
            _ParsingTable = t;
        }
    }
}
