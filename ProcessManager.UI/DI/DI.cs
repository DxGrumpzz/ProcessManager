namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Simple DI container
    /// </summary>
    public static class DI
    {

        /// <summary>
        /// A service provider
        /// </summary>
        public static IServiceProvider Provider { get; set; }


        /// <summary>
        /// The process list file loader/reader
        /// </summary>
        public static IProcessLoader ProcessLoader => GetService<IProcessLoader>();
        

        /// <summary>
        /// A project loader, Used to load project data from file
        /// </summary>
        public static IProjectLoader ProjectLoader => GetService<IProjectLoader>();


        /// <summary>
        /// The list of projects loaded from file
        /// </summary>
        public static IList<Project> Projects => GetService<IList<Project>>();


        /// <summary>
        /// The app's main viewmodel
        /// </summary>
        public static MainWindowViewModel MainWindowViewModel => GetService<MainWindowViewModel>();


        /// <summary>
        /// a <see cref="UI.SystemTrayIcon"/> used to interact with the app's associated TrayIcon
        /// </summary>
        public static SystemTrayIcon SystemTrayIcon => GetService<SystemTrayIcon>();


        /// <summary>
        /// An OS folder dialog. Used to select project directories
        /// </summary>
        public static IFolderDialog FolderDialog => GetService<IFolderDialog>(); 


        /// <summary>
        /// An OS file dialog, Used to select processes to run, and such
        /// </summary>
        public static IFileDialog FileDialog => GetService<IFileDialog>(); 
        

        /// <summary>
        /// A User dialog. Used for: asking auser to make a decision, Such as; 
        /// if to include an existing project directory to app
        /// </summary>
        public static IUserDialog UserDialog => GetService<IUserDialog>();


        /// <summary>
        /// The app's serializer
        /// </summary>
        public static ISerializer Serializer => GetService<ISerializer>();
        

        /// <summary>
        /// The app's UI manager, responsible for changing views, and such
        /// </summary>
        public static IUIManager UI => GetService<IUIManager>();


        /// <summary>
        /// A logger. Used to display build info, errors, and warnings
        /// </summary>
        public static ILogger Logger => GetService<ILogger>();


        public static ProjectsListViewModel ProjectsListVM => GetService<ProjectsListViewModel>();


        /// <summary>
        /// A shortcut for GetService function in IServiceProvider
        /// </summary>
        /// <typeparam name="TService"> The service to find </typeparam>
        /// <returns></returns>
        private static TService GetService<TService>()
        {
            var service = Provider.GetService(typeof(TService));
            return (TService)service;
        }

    };
};