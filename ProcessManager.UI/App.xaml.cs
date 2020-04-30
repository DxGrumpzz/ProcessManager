namespace ProcessManager.UI
{
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text.Json;
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

            // Pre load DI stuff necessary to do validation, error check, and such
            var serviceCollection = PreLoadDI();


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


            // Setup DI stuff
            serviceCollection = SetupDI(serviceCollection);

            DI.Provider = BuildDIProvider(serviceCollection);

            // Create and show window
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();

            // Build the icon
            var hwnd = new WindowInteropHelper(Current.MainWindow).Handle;
            DI.SystemTrayIcon.BuildIcon(Localization.APP_ICON_LOCATION, hwnd);
            DI.SystemTrayIcon.ShowIcon();
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


        private IServiceCollection PreLoadDI()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IUserDialog>((ServiceProvider) => new WindowsUserDialog());

            return serviceCollection;
        }

        private IServiceCollection SetupDI(IServiceCollection serviceCollection)
        {
            // Setup serializer
            serviceCollection.AddTransient<ISerializer>((ServiceProvider) =>
            new JsonSerializer(
                new JsonSerializerOptions()
                {
                    IgnoreNullValues = true,
                    WriteIndented = true,
                }));


            // Add folder dialog 
            serviceCollection.AddScoped<IFolderDialog>((serviceProvider) => new WindowsFolderDialog());

            // Add file dialog
            serviceCollection.AddScoped<IFileDialog>((serviceProvider) => new WindowsFileDialog());

            // Create a process loader
            serviceCollection.AddScoped<IProcessLoader>((serviceProvider) => new ProcessLoader(serviceProvider.GetService<ISerializer>()));


            // Setup project loader
            serviceCollection.AddScoped<IProjectLoader>((serviceProvider) =>
            new ProjectLoader(
                    processLoader:         serviceProvider.GetService<IProcessLoader>(),
                    serializer:            serviceProvider.GetService<ISerializer>(),
                    projectsFilename:      Localization.PROJECTS_FILE_NAME,
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
            });


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