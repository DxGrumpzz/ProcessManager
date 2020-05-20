namespace ProcessManager.UI
{
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Interop;


    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

#if DEBUG
        /// <summary>
        /// A console that will be displayed 
        /// </summary>
        [DllImport("Kernel32.dll")]
        private static extern void AllocConsole();
#endif


        private const string LOG_FILE_NAME = "Log.txt";

        private string _logFilepath = $"{Environment.CurrentDirectory}\\{LOG_FILE_NAME}";



        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Pre load DI stuff necessary to do validation, error check, and such
            var serviceCollection = SetupDI();

            DI.Provider = BuildDIProvider(serviceCollection);


            // Check if the processes file exists
            // If Projects file doesn't exist
            if (File.Exists(Localization.PROJECTS_FILE_NAME) == false)
            {
                // Build the service provider
                serviceCollection.BuildServiceProvider()
                    // Dispaly an error message to the user
                    .GetService<IUserDialog>().ShowDialog($"Unable to find {Localization.PROJECTS_FILE_NAME}");

                // Exit application
                Environment.Exit(1);
                return;
            };

            // Create new file for logging
            File.Create(_logFilepath)
                // Dispose file stream after creation
                .Dispose();

            DI.Logger.Log("Creating MainWindow...");

            // Create and show window
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();


            DI.Logger.Log("Building tray icon...");

            // Build the icon
            var hwnd = new WindowInteropHelper(Current.MainWindow).Handle;
            DI.SystemTrayIcon.BuildIcon(Localization.APP_ICON_LOCATION, hwnd);
            DI.SystemTrayIcon.ShowIcon();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DI.SystemTrayIcon.RemoveIcon();

            Task.Run(() =>
            {
                // Close the processes when app exists
                foreach (var project in DI.Projects)
                {
                    Task.Run(project.CloseProject);
                };
            });
        }

        private IServiceCollection SetupDI()
        {
            var serviceCollection = new ServiceCollection();


            serviceCollection.AddTransient<IUserDialog, WindowsUserDialog>();


#if DEBUG
            AllocConsole();
            Console.Title = "ProcessManager.UI console logger";

            serviceCollection.AddTransient<ILogger, ConsoleLogger>();
#else
            serviceCollection.AddTransient<ILogger, FileLogger>();
#endif


            // Setup serializer
            serviceCollection.AddTransient<ISerializer, JsonSerializer>();

            // Add folder dialog 
            serviceCollection.AddScoped<IFolderDialog, WindowsFolderDialog>();

            // Add file dialog
            serviceCollection.AddScoped<IFileDialog, WindowsFileDialog>();

            // Create a process loader
            serviceCollection.AddScoped<IProcessLoader>((serviceProvider) => new ProcessLoader(serviceProvider.GetService<ISerializer>()));


            // Setup project loader
            serviceCollection.AddScoped<IProjectLoader>((serviceProvider) =>
            new ProjectLoader(
                    processLoader: serviceProvider.GetService<IProcessLoader>(),
                    serializer: serviceProvider.GetService<ISerializer>(),
                    logger: serviceProvider.GetService<ILogger>(),
                    projectsFilename: Localization.PROJECTS_FILE_NAME,
                    projectConfigFilename: Localization.PROJECT_CONFIG_FILE_NAME));


            // Load the projects list
            serviceCollection.AddSingleton<IList<Project>>((serviceProvider) =>
            {
                var projectLoader = serviceProvider.GetService<IProjectLoader>();

                // Load the projects
                projectLoader.LoadProjectsDirectories();
                projectLoader.ValidateLoadedProjects();
                projectLoader.LoadProjectProcesses();

                return new List<Project>(projectLoader.GetProjectsList());
            });


            // Setup main viewmodel
            serviceCollection.AddSingleton((serviceProvider) => new MainWindowViewModel()
            {
                CurrentView = new ProjectListView(
                    new ProjectsListViewModel(DI.Projects)),
                // CurrentView = new AddConsoleProcessView(new AddConsoleProcessViewModel(new ProjectItemViewModel(DI.Projects[0]))),
            });

            serviceCollection.AddTransient<IUIManager, UImanager>();

            // Setup TrayIcon 
            serviceCollection.AddSingleton((serviceProvider) =>
            new SystemTrayIcon(
                () => serviceProvider.GetService<IList<Project>>()
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
                .ToArray()));


            return serviceCollection;
        }

        private IServiceProvider BuildDIProvider(IServiceCollection serviceCollection)
        {
            var serviceProvider = serviceCollection.BuildServiceProvider();

            return serviceProvider;
        }

    };
};