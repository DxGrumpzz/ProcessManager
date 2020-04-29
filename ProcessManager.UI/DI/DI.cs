namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Simple DI container
    /// </summary>
    public static class DI
    {
        public static IServiceProvider Provider { get; set; }


        /// <summary>
        /// The process list file loader/reader
        /// </summary>
        public static IProcessLoader ProcessLoader => GetService<IProcessLoader>();

        public static IList<Project> Projects => GetService<IList<Project>>();

        public static IProjectLoader ProjectLoader => GetService<IProjectLoader>();

        public static MainWindowViewModel MainWindowViewModel => GetService<MainWindowViewModel>();

        public static SystemTrayIcon SystemTrayIcon => GetService<SystemTrayIcon>();


        public static IFolderDialog FolderDialog => GetService<IFolderDialog>(); 
        public static IFileDialog FileDialog => GetService<IFileDialog>(); 

        public static IUserDialog UserDialog => GetService<IUserDialog>();


        private static TService GetService<TService>()
        {
            var service = Provider.GetService(typeof(TService));
            return (TService)service;
        }

    };
};
