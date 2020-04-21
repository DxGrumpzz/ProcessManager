namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Check if the processes file exists
            if (File.Exists(Localization.PROJECTS_FILE_NAME) == false)
            {
                MessageBox.Show($"Unable to find {Localization.PROJECTS_FILE_NAME}");

                // Exit application
                Environment.Exit(1);
                return;
            };

            // Setup DI stuff
            SetupDI();

            
            // Create and show window
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();

            // Setup TrayIcon stuff
            // This is here and not in SetupDI because SystemTrayIcon takes an HWND as a parameter, so we need a valid window to initialize this class
            DI.SetupTrayIcon(new WindowInteropHelper(Current.MainWindow).Handle);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DI.SystemTrayIcon.RemoveIcon();

            // Close the processes when app exists
            foreach (var project in DI.Projects)
            {
                project.CloseProjectTree();
            };
        }

        private static void SetupDI()
        {
            // Create a process loader
            DI.ProcessLoader = new JsonProcessLoader();

            // Setup project loader
            var projectLoader = DI.ProjectLoader = new ProjectLoader(
                processLoader: DI.ProcessLoader,
                filename: Localization.PROJECTS_FILE_NAME,
                projectConfigFilename: Localization.PROJECT_CONFIG_FILE_NAME);

            // Load the projects
            projectLoader.LoadProjectsDirectories();
            projectLoader.ValidateLoadedProjects();
            projectLoader.LoadProjectProcesses();


            // Load the projects list into DI
            DI.Projects = new List<Project>(projectLoader.GetProjectsList());


            DI.MainWindowViewModel = new MainWindowViewModel()
            {
                CurrentView = new ProjectListView(
                   new ProjectsListViewModel(DI.Projects)),
            };
        }
    };
};