﻿namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
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
            .Select(process => new ProcessItemViewModel(process)).ToArray();


        #region Public commands

        public ICommand GotoMainPageCommnad { get; }
        public ICommand CloseProjectCommand { get; }
        public ICommand RunProjectCommand { get; }

        public ICommand AddNewProcessCommand { get; }
        public ICommand AddNewConsoleProcessCommand { get; }

        public ICommand DeleteProjectCommand { get; }

        #endregion


        public ProjectItemViewModel(Project project)
        {
            Project = project;

            GotoMainPageCommnad = new RelayCommand(ExecuteGotoMainPageommnad);

            CloseProjectCommand = new RelayCommand(ExecuteCloseProjectCommand);
            RunProjectCommand = new RelayCommand(ExecuteRunProjectCommand);

            AddNewProcessCommand = new RelayCommand(ExecuteAddNewProcessCommand);
            AddNewConsoleProcessCommand = new RelayCommand(ExecuteAddNewConsoleProcessCommand);

            DeleteProjectCommand = new RelayCommand(ExecuteDeleteProjectCommand);
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

            // Convert the 'new' projects list to json objects
            var projectsAsJson = DI.Projects
                .Select(project =>
                new
                {
                    ProjectPath = project.ProjectPath,
                });

            // Convert the json object to json string
            string jsonString = JsonSerializer.Serialize(projectsAsJson);

            // Write the json string to Projects file
            File.WriteAllText(Localization.PROJECTS_FILE_NAME, jsonString);

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

        private void ExecuteRunProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                process.RunProcess();
            };
        }

        private void ExecuteCloseProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                process.CloseProcess();
            };
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