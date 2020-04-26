namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Simple DI container
    /// </summary>
    public static class DI
    {
        /// <summary>
        /// The process list loaded from file
        /// </summary>
        public static List<IProcessModel> ProcessList { get; set; }

        /// <summary>
        /// The process list file loader/reader
        /// </summary>
        public static IProcessLoader ProcessLoader { get; set; }

        public static IList<Project> Projects { get; set; }

        public static IProjectLoader ProjectLoader { get; set; }

        public static MainWindowViewModel MainWindowViewModel { get; set; }

        public static SystemTrayIcon SystemTrayIcon { get; set; }


        public static IFolderDialog FolderDialog => new WindowsFolderDialog();
        public static IFileDialog FileDialog => new WindowsFileDialog();



        /// <summary>
        /// Creates a TrayIcon, and send it some data
        /// </summary>
        /// <param name="hwnd"></param>
        public static void SetupTrayIcon(IntPtr hwnd)
        {
            // Create the SystemTrayIcon class
            SystemTrayIcon = new SystemTrayIcon(Localization.APP_ICON_LOCATION, hwnd);

            // Initialize the tray icon and give it the neccessary project data
            SystemTrayIcon.CreateIcon(Projects
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
            SystemTrayIcon.ShowIcon();
        }

    };
};
