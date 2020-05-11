namespace ProcessManager.UI
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ProjectListView : UserControl
    {
        public ProjectListView()
        {
            InitializeComponent();
        }

        public ProjectListView(ProjectsListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }

    }
}
