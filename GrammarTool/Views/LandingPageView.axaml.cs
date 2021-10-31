using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GrammarTool.Views
{
    public partial class LandingPageView : UserControl
    {
        public LandingPageView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
