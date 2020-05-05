namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for ProjectListItemView.xaml
    /// </summary>
    public partial class ProjectListItemView : UserControl
    {

        public ProjectListItemView()
        {
            InitializeComponent();
        }

        public ProjectListItemView(ProjectListItemViewModel viewModel) :
            this()
        {
            DataContext = viewModel;
        }


    };
};