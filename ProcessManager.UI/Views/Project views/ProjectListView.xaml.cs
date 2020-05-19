namespace ProcessManager.UI
{
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ProjectListView : UserControl
    {
        private ProjectsListViewModel _viewModel;

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


        private void Button_DragEnter(object sender, DragEventArgs e)
        {
            var button = (Button)sender;

            DI.Logger.Log("Drag enter");


            Debugger.Break();
        }

        private void MainBorder_Drop(object sender, DragEventArgs e)
        {
            DI.Logger.Log("Drag, dropped");

            Border border = (Border)sender;
            var currentData = (ProjectListItemViewModel)border.DataContext;

            var data = e.Data.GetData("Data1");

            if (!(data is ProjectListItemViewModel droppedData))
            {
                Debugger.Break();
                return;
            };


            var currentIndex = DI.Projects.IndexOf(currentData.Project);
            var droppedIndex = DI.Projects.IndexOf(droppedData.Project);

            var temp3 = DI.Projects[currentIndex];

            DI.Projects[currentIndex] = droppedData.Project;
            DI.Projects[droppedIndex] = temp3;

            var bytes = DI.Serializer.SerializerProjects(DI.Projects);
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, bytes);

            _viewModel.Projects = new ObservableCollection<ProjectListItemViewModel>(DI.Projects.Select(project =>
            {
                return new ProjectListItemViewModel(project);
            }));
        }


        private void MainBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DI.Logger.Log("Drag started");

                Border border = (Border)sender;

                DataObject dragDropData = new DataObject();

                dragDropData.SetData("Data1", border.DataContext);

                DragDrop.DoDragDrop(border, dragDropData, DragDropEffects.Move);
            };
        }
    }
}
