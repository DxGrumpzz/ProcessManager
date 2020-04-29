namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Windows.Input;
    using System.Windows.Interop;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectsListViewModel : BaseViewModel
    {

        public static ProjectsListViewModel DesignInstance => new ProjectsListViewModel()
        {
            Projects = new ObservableCollection<ProjectItemViewModel>()
            {
                new ProjectItemViewModel(new Project()
                {
                    ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                }),
                new ProjectItemViewModel(new Project()
                {
                    ProjectPath = $@"D:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                }),
                new ProjectItemViewModel(new Project()
                {
                    ProjectPath = $@"A:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                    ProcessList = new List<IProcessModel>(),
                }),
            },
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ObservableCollection<ProjectItemViewModel> _projects;

        #endregion


        #region Public properties

        /// <summary>
        /// A list of projects
        /// </summary>
        public ObservableCollection<ProjectItemViewModel> Projects
        {
            get => _projects;
            set
            {
                _projects = value;
                OnPropertyChanged();
            }
        }

        #endregion


        #region Public commands

        public ICommand AddNewProjectCommnad { get; }

        #endregion



        /// <summary>
        /// The only reason for this constructor to exist is so the DesignTime singelton would actually work in the designer
        /// </summary>
        private ProjectsListViewModel() { }

        public ProjectsListViewModel(IEnumerable<Project> projects)
        {
            Projects = new ObservableCollection<ProjectItemViewModel>(projects.Select(project =>
            {
                return new ProjectItemViewModel(project);
            }));

            AddNewProjectCommnad = new RelayCommand(ExecuteAddNewProjectCommnad);
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
            var jsonString = JsonSerializer.Serialize(DI.Projects.Select(project => new
            {
                ProjectPath = project.ProjectPath,
            }),
            new JsonSerializerOptions()
            {
                WriteIndented = true,
            });

            // Write the json to file
            File.WriteAllText(Localization.PROJECTS_FILE_NAME, jsonString);

            // Update tray icon
            DI.SystemTrayIcon.RebuildIcon(showAfterBuild: true);

            // Add the project to this ViewModel's projects list
            Projects.Add(new ProjectItemViewModel(newProject));
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