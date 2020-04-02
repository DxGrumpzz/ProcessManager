namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text.Json;
    using System.Windows;
    using System.Windows.Interop;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const string PROJECT_CONFIG_FILE_NAME = "ProcessManger.Config.Json";
         private const string PROJCES_FILE_NAME = "Projects.json";

        [DllImport("ProcessManager.Core.Dll")]
        private extern static IntPtr CreateSystemTrayIcon(IntPtr mainWindowHandle);
        [DllImport("ProcessManager.Core.Dll")]
        private extern static void RemoveSystemTrayIcon(IntPtr iconPointer);

        IntPtr _iconPointer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            // Setup file process loader

            IProjectLoader projectLoader = new ProjectLoader(
                filename: PROJCES_FILE_NAME, 
                projectConfigFilename: PROJECT_CONFIG_FILE_NAME);


            projectLoader.ProjectsFileExists();

            projectLoader.LoadProjectsDirectories();

            projectLoader.ValidateLoadedProjects();

            projectLoader.LoadProjectProcesses();
            
            DI.Projects = projectLoader.GetProjectsList();


            LoadProjectProcesses(DI.Projects);

            DI.MainWindowViewModel = new MainWindowViewModel(DI.Projects);


            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();

            _iconPointer = CreateSystemTrayIcon(new WindowInteropHelper(Current.MainWindow).Handle);

            // Debugger.Break();
            //RemoveSystemTrayIcon(iconPointer);

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
            RemoveSystemTrayIcon(_iconPointer);

            // Close the processes when app exists
            foreach (var project in DI.Projects)
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
