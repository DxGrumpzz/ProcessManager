namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const string PROJECT_CONFIG_FILE_NAME = "ProcessManger.Config.Json";
        private const string PROCESS_MANAGER_PROJCES_FILE_NAME = "Projects.json";


        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Setup file process loader

            DI.Projects = LoadProjectsDirectories(PROCESS_MANAGER_PROJCES_FILE_NAME);

            ValidateLoadedProjects(DI.Projects);

            LoadProjectProcesses(DI.Projects);

            DI.MainWindowViewModel = new MainWindowViewModel(DI.Projects);

            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
                .Show();

            //// Check if the processes file exists
            //if (File.Exists("ProcessList.json") == false)
            //{
            //    MessageBox.Show("Unable to find ProcessList.json");

            //    // Exit application if it isn't
            //    Environment.Exit(1);
            //    return;
            //};

            //// Setup DI stuff
            //SetupDI();


            //// Create the main window
            //(Current.MainWindow = new MainWindow(
            //    new MainWindowViewModel(DI.ProcessList)))
            //    .Show();
        }


        private IEnumerable<Project> LoadProjectsDirectories(string filename)
        {
            return JsonSerializer.Deserialize<IEnumerable<Project>>(File.ReadAllBytes(filename));
        }

        private void ValidateLoadedProjects(IEnumerable<Project> loadedProjects)
        {
            foreach (var project in loadedProjects)
            {
                if (Directory.Exists(project.ProjectPath) == false)
                {
                    throw new Exception($"{project.ProjectPath} is not a valid directory or it doesn't exist");
                };

                if (File.Exists(Path.Combine(project.ProjectPath, PROJECT_CONFIG_FILE_NAME)) == false)
                {
                    throw new Exception($"{project.ProjectPath} doesn't contain a \"ProcessManger.Config.Json\" file");
                };

            };
        }

        private void LoadProjectProcesses(IEnumerable<Project> projects)
        {
            foreach (var project in projects)
            {
                try
                {
                    project.ProcessList = JsonSerializer.Deserialize<IEnumerable<ProcessModel>>(File.ReadAllBytes(project.ProjectPathWithConfig));
                }
                catch (JsonException jsonException)
                {
                    throw new Exception($"Failed to read {project.ProjectPathWithConfig}, File doesn't contain valid json data");
                };
            };
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // Close the processes when app exists
            foreach(var project in DI.Projects)
            {
                foreach(var process in project.ProcessList)
                {
                    process.CloseProcessTree();
                };
            };
        }


        private static void SetupDI()
        {
            // Setup file process loader
            DI.ProcessLoader = new JsonProcessLoader("ProcessList.json");

            // Check if process list file is valid
            var processList = DI.ProcessLoader.GetProcessListFromFile();
            if (processList == null)
            {
                MessageBox.Show("ProcessList.json contains invalid data", "Error");

                // Exit application if it isn't
                Environment.Exit(1);
                return;
            };

            // Setup process list
            DI.ProcessList = new List<ProcessModel>(processList);
        }

    };
};
