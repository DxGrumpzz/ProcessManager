﻿namespace ProcessManager.UI
{
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
            string selectedFolder = DI.FolderDialog.ShowDialog();

            if (string.IsNullOrWhiteSpace(selectedFolder))
                return;

            string configPath = Path.Combine(selectedFolder, Localization.PROJECT_CONFIG_FILE_NAME);

            // If folder already contains Project.Config file
            if (File.Exists(configPath))
                return;

            // Create Project.Config 
            File.Create(configPath).Dispose();

            // Write empty json brackets to file
            File.WriteAllText(configPath, "[\n\n]");

            // Create the new project
            var newProject = new Project()
            {
                ProjectPath = selectedFolder,
                ProcessList = new List<IProcessModel>(),
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


            // Add the project to this ViewModel's projects list
            Projects.Add(new ProjectItemViewModel(newProject));

            // Update tray icon
            DI.SystemTrayIcon.RemoveIcon();
            DI.SetupTrayIcon(new WindowInteropHelper(App.Current.MainWindow).Handle);
        }

    };
};