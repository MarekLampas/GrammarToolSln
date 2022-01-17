using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GrammarTool.Views
{
    public partial class ScannerPanelView : UserControl
    {
        public ScannerPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
