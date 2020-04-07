namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string PROJECT_CONFIG_FILE_NAME = "ProcessManger.Config.Json";
        private const string PROJCES_FILE_NAME = "Projects.json";

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

            // Create and show window
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();

            // Setup TrayIcon stuff
            SetupTrayIcon();
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
                filename: PROJCES_FILE_NAME,
                projectConfigFilename: PROJECT_CONFIG_FILE_NAME);

            // Load the projects
            projectLoader.LoadProjectsDirectories();
            projectLoader.ValidateLoadedProjects();
            projectLoader.LoadProjectProcesses();


            // Load the projects list into DI
            DI.Projects = projectLoader.GetProjectsList();

            DI.MainWindowViewModel = new MainWindowViewModel(DI.Projects);
        }

        private void SetupTrayIcon()
        {
            // This is here and not in SetupDI because SystemTrayIcon takes an HWND as a parameter so we need a valid window to initialize this class
            // Create the SystemTrayIcon class
            DI.SystemTrayIcon = new SystemTrayIcon("Resources\\Icon.ico", new WindowInteropHelper(Current.MainWindow).Handle);

            // Initialize the tray icon and give it the neccessary project data
            DI.SystemTrayIcon.CreateIcon(DI.Projects
            .Select(project =>
            {
                // The data which will be passed to the tray icon
                var trayIconData = new SystemTrayIconData(project);

                // A local function that will take a handle and "convert" it to a usable object
                static T HandleToObj<T>(IntPtr handle)
                {
                    // Take the handle and convert it to a *safe handle
                    if (GCHandle.FromIntPtr(handle).Target is T obj)
                        return obj;
                    else
                    {
                        Debugger.Break();
                        throw new Exception("Critical error occured. Unable to read returned TrayIcon data");
                    };
                };

                // A callback that will be called when the user decided to close the project
                trayIconData.CloseProjectCallBack += (data) =>
                {
                    Project project = HandleToObj<Project>(data);
                    project.CloseProjectTree();
                };

                // A callback that will be called when the user decided to run the project
                trayIconData.RunProjectCallBack += (data) =>
                {
                    Project project = HandleToObj<Project>(data);
                    project.RunProject();
                };

                return trayIconData;

            })
            // Convert to an array Because
            .ToArray());


            // Actually show the icon
            DI.SystemTrayIcon.ShowIcon();
        }

    };
};
