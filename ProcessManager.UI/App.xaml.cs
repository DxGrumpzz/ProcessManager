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


            // Check if the processes file exists
            if (File.Exists(PROJCES_FILE_NAME) == false)
            {
                MessageBox.Show($"Unable to find {PROJCES_FILE_NAME}");

                // Exit application
                Environment.Exit(1);
                return;
            };

            // Setup DI stuff
            SetupDI();


            DI.MainWindowViewModel = new MainWindowViewModel(DI.Projects);
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();

            _iconPointer = CreateSystemTrayIcon(new WindowInteropHelper(Current.MainWindow).Handle);
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
            // Create a process loader
            DI.ProcessLoader = new JsonProcessLoader();

            // Setup project loader
            DI.ProjectLoader = new ProjectLoader(
                processLoader: DI.ProcessLoader,
                filename: PROJCES_FILE_NAME,
                projectConfigFilename: PROJECT_CONFIG_FILE_NAME);

            // Load the projects
            DI.ProjectLoader.LoadProjectsDirectories();
            DI.ProjectLoader.ValidateLoadedProjects();
            DI.ProjectLoader.LoadProjectProcesses();
            

            // Load the projects list into DI
            DI.Projects = DI.ProjectLoader.GetProjectsList();
        }

    };
};
