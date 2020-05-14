namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectItemViewModel
    {
        public static ProjectItemViewModel DesignInstance => new ProjectItemViewModel(new Project()
        {
            ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",

            ProcessList = new IProcessModel[]
            {
                new GUIProcess($@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.exe"),

                new GUIProcess($@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.bat"),

                new ConsoleProcess("npm run start" ,$@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.bat"),
            },
        });


        /// <summary>
        /// The project associated with this viewmodel
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// The project's processes as a "list" of ProcessItemViewModel
        /// </summary>
        public IEnumerable<ProcessItemViewModel> ProcessList => Project.ProcessList
            .Select(process => new ProcessItemViewModel(this, process)).ToArray();


        #region Public commands

        public ICommand GotoMainPageCommnad { get; }

        public ICommand AddNewProcessCommand { get; }
        public ICommand AddNewConsoleProcessCommand { get; }

        public ICommand DeleteProjectCommand { get; }

        public ICommand OpenProjectDirectoryCommand { get; }

        #endregion


        public ProjectItemViewModel(Project project)
        {
            Project = project;

            GotoMainPageCommnad = new RelayCommand(ExecuteGotoMainPageommnad);

            AddNewProcessCommand = new RelayCommand(ExecuteAddNewProcessCommand);
            AddNewConsoleProcessCommand = new RelayCommand(ExecuteAddNewConsoleProcessCommand);

            DeleteProjectCommand = new RelayCommand(ExecuteDeleteProjectCommand);

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(Project.ProjectPath));
        }



        private void ExecuteDeleteProjectCommand()
        {
            var userDialog = DI.UserDialog;

            // Ask for user confirmation
            var result = userDialog.ShowChoiceDialog($"Are you absolutley sure you want to delete \'{Project.ProjectName}\' from your project list ?", "Delete project confirmation");

            // Don't do anything if user didn't confirm the deletion
            if (result != UserDialogResult.Yes)
                return;

            // Remove this project from from list in DI
            DI.Projects.Remove(Project);

            // Convert the json object to json string
            var jsonBytes = DI.Serializer.SerializerProjects(DI.Projects);

            // Write the json string to Projects file
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, jsonBytes);

            // Switch back to main page
            SwitchToProjectListView();
        }

        private void ExecuteAddNewConsoleProcessCommand()
        {
            DI.MainWindowViewModel.CurrentView = new AddConsoleProcessView(new AddConsoleProcessViewModel(this));
        }

        private void ExecuteAddNewProcessCommand()
        {
            DI.MainWindowViewModel.CurrentView = new AddProcessView(new AddProcessViewModel(this));
        }



        private void ExecuteGotoMainPageommnad()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

        private void SwitchToProjectListView()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

    };
};