namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectItemViewModel : BaseViewModel
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

        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<ProcessItemViewModel> _processList;

        #endregion

        /// <summary>
        /// The project associated with this viewmodel
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// The project's processes as a "list" of ProcessItemViewModel
        /// </summary>
        public ObservableCollection<ProcessItemViewModel> ProcessList
        {
            get => _processList;
            private set
            {
                _processList = value;
                OnPropertyChanged();
            }
        }



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

            UpdateProcessList();

            GotoMainPageCommnad = new RelayCommand(ExecuteGotoMainPageommnad);

            AddNewProcessCommand = new RelayCommand(ExecuteAddNewProcessCommand);
            AddNewConsoleProcessCommand = new RelayCommand(ExecuteAddNewConsoleProcessCommand);

            DeleteProjectCommand = new RelayCommand(ExecuteDeleteProjectCommand);

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(Project.ProjectPath));
        }

        public void UpdateProcessList()
        {
            ProcessList = new ObservableCollection<ProcessItemViewModel>(Project.ProcessList
            .Select(process => new ProcessItemViewModel(this, process)));
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

            // Update projects list after project deletion
            DI.ProjectsListVM.UpdateProjectsList();

            // Update projects list
            DI.FileManager.UpdateProjectsList(DI.Projects);

            // Switch back to main page
            SwitchToProjectListView();
        }

        private void ExecuteAddNewConsoleProcessCommand()
        {
            DI.UI.ChangeView(View.AddConsoleProcessView, new AddConsoleProcessViewModel(this));
        }

        private void ExecuteAddNewProcessCommand()
        {
            DI.UI.ChangeView(View.AddProcessView, new AddProcessViewModel(this));
        }



        private void ExecuteGotoMainPageommnad()
        {
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM);
        }

        private void SwitchToProjectListView()
        {
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM);
        }

    };
};