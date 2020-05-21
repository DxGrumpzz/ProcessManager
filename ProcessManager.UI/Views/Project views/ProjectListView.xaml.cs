namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

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
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
