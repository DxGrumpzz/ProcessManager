namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectsListViewModel : BaseViewModel
    {

        public static ProjectsListViewModel DesignInstance => new ProjectsListViewModel()
        {
            Projects = new ObservableCollection<ProjectListItemViewModel>()
            {
                new ProjectListItemViewModel(new Project()
                {
                    ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                })
                {
                    SettingsButtonVisible = true,
                },
                new ProjectListItemViewModel(new Project()
                {
                    ProjectPath = $@"D:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                }),
                new ProjectListItemViewModel(new Project()
                {
                    ProjectPath = $@"A:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                }),
            },
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<ProjectListItemViewModel> _projects;

        #endregion


        #region Public properties

        /// <summary>
        /// A list of projects
        /// </summary>
        public ObservableCollection<ProjectListItemViewModel> Projects
        {
            get => _projects;
            private set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }

        #endregion



        public ICommand AddNewProjectCommnad { get; }



        /// <summary>
        /// The only reason for this constructor to exist is so the DesignTime singelton would actually work in the designer
        /// </summary>
        private ProjectsListViewModel() { }

        public ProjectsListViewModel(IEnumerable<Project> projects)
        {
            Projects = new ObservableCollection<ProjectListItemViewModel>(projects.Select(project =>
            {
                return new ProjectListItemViewModel(project);
            }));

            AddNewProjectCommnad = new RelayCommand(ExecuteAddNewProjectCommnad);
        }


        /// <summary>
        /// Refreshes the Projects list using the Projects provided in <see cref="DI.Projects"/>
        /// </summary>
        public void UpdateProjectsList()
        {
            // Update projects list
            DI.ProjectsListVM.Projects = new ObservableCollection<ProjectListItemViewModel>(DI.Projects
                // Convert ProjectModel to ProjectListItemViewModel
                .Select(project => new ProjectListItemViewModel(project)));
        }

        private void ExecuteAddNewProjectCommnad()
        {
            var folderDialog = DI.FolderDialog;

            // Show select folder dialog
            var selectedFolderResult = folderDialog.ShowDialog();

            // If user canceled selection
            if (selectedFolderResult == false)
                return;

            // Combine the selected path and the config file name to get the path to the new config file
            string configPath = Path.Combine(folderDialog.SelectedPath, Localization.PROJECT_CONFIG_FILE_NAME);

            // If folder already contains Project.Config file
            if (File.Exists(configPath))
            {
                // Ask user if he'd like to add the project to the app
                var result = DI.UserDialog.ShowChoiceDialog($"Selected folder already contains a \'{Localization.PROJECT_CONFIG_FILE_NAME}\' file.\n" +
                                                            $"Would you like to add it ?");

                // If user clicked yes
                if (result == UserDialogResult.Yes)
                {
                    // Add the project to app
                    AddProjectToApp(folderDialog.SelectedPath, configPath);
                };
            }
            else
            {
                // Create empty config file
                CreateEmptyConfigFile(configPath);

                // Add project to app
                AddProjectToApp(folderDialog.SelectedPath, configPath);
            };

        }


        /// <summary>
        /// Adds a Project's config file to the app
        /// </summary>
        /// <param name="selectedPath"> A path to a project directory that the user has selected </param>
        /// <param name="configPath"> A path that combines the selected path and the filename of the config file </param>
        private void AddProjectToApp(string selectedPath, string configPath)
        {
            // Read process info from config file
            var processList = DI.ProcessLoader.GetProcessListFromFile(configPath);

            // Create the new project
            var newProject = new Project()
            {
                ProjectPath = selectedPath,
                ProcessList = new List<IProcessModel>(processList),
            };

            // Add the new project to the projects list
            DI.Projects.Add(newProject);


            // Convert the new projects list to a json string
            var jsonBytes = DI.Serializer.SerializerProjects(DI.Projects);

            // Write the json to file
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, jsonBytes);

            // Update tray icon
            DI.SystemTrayIcon.RebuildIcon(showAfterBuild: true);

            // Add the project to this ViewModel's projects list
            Projects.Add(new ProjectListItemViewModel(newProject));
        }


        /// <summary>
        /// Create an empty <see cref="Localization.PROJECT_CONFIG_FILE_NAME"/>
        /// </summary>
        /// <param name="configPath"></param>
        private void CreateEmptyConfigFile(string configPath)
        {
            // Create Project.Config 
            File.Create(configPath).Dispose();

            // Write empty json brackets to file
            File.WriteAllText(configPath, "[\n\n]");
        }



    };
};