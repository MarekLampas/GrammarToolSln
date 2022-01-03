using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GrammarTool.Views
{
    public partial class MainWindowView : Window
    {
        public static MainWindowView Instance { get; private set; }
        public MainWindowView()
        {
            Instance = this;
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
