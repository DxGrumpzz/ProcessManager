namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectItemView : UserControl
    {
        public ProjectItemView(ProjectItemViewModel projectItemViewModel)
        {
            DataContext = projectItemViewModel;
            InitializeComponent();
        }
    }
}
