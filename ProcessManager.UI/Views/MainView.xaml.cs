namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();
        }

        public MainView(MainWindowViewModel mainWindowViewModel) :
            this()
        {
            DataContext = mainWindowViewModel;
        }
    }
}
