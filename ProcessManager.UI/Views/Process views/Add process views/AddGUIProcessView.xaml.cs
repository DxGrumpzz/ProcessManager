namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AddProcessView.xaml
    /// </summary>
    public partial class AddGUIProcessView : UserControl
    {
        public AddGUIProcessView()
        {
            InitializeComponent();
        }

        public AddGUIProcessView(AddGUIProcessViewModel viewModel) 
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
