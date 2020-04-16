namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ProjectsView : UserControl
    {
        public ProjectsView()
        {
            InitializeComponent();
        }

        public ProjectsView(ProjectsListViewModel viewModel) :
            this()
        {
            DataContext = viewModel;
        }
    }
}
