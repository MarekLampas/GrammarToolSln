using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GrammarTool.Views
{
    public partial class GrammarPanelView : UserControl
    {
        public GrammarPanelView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
