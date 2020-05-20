namespace ProcessManager.UI
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ProjectListView : UserControl
    {
        private ProjectsListViewModel _viewModel;

        private const string DRAG_DROP_DATA_NAME = "ViewModelData";

        public ProjectListView()
        {
            InitializeComponent();
        }

        public ProjectListView(ProjectsListViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
            _viewModel = viewModel;
        }


        private void MainBorder_Drop(object sender, DragEventArgs e)
        {
            Border border = (Border)sender;
            var currentData = (ProjectListItemViewModel)border.DataContext;

            var data = e.Data.GetData(DRAG_DROP_DATA_NAME);

            if (data is null)
                return;

            if (!(data is ProjectListItemViewModel droppedData))
            {
                Debugger.Break();
                return;
            };

            if (currentData == data)
                return;


            var currentIndex = DI.Projects.IndexOf(currentData.Project);
            var droppedIndex = DI.Projects.IndexOf(droppedData.Project);

            var temp = DI.Projects[currentIndex];

            DI.Projects[currentIndex] = droppedData.Project;
            DI.Projects[droppedIndex] = temp;

            var bytes = DI.Serializer.SerializerProjects(DI.Projects);
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, bytes);


            _viewModel.Projects = new ObservableCollection<ProjectListItemViewModel>(DI.Projects.Select(project =>
            {
                return new ProjectListItemViewModel(project);
            }));
        }


        private void Button_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Button border = (Button)sender;

                DataObject dragDropData = new DataObject();

                dragDropData.SetData(DRAG_DROP_DATA_NAME, border.DataContext);

                DragDrop.DoDragDrop(border, dragDropData, DragDropEffects.Move);
            };
        }

        private void MainBorder_DragEnter(object sender, DragEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = (SolidColorBrush)App.Current.Resources["LightGreyBrush"];
        }

        private void MainBorder_DragLeave(object sender, DragEventArgs e)
        {
            Border border = (Border)sender;
            border.Background = Brushes.Transparent;
        }
    }
}
