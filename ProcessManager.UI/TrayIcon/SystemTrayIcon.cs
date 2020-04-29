namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// A class responsible for interaction with the TrayIcon
    /// </summary>
    public class SystemTrayIcon
    {

        #region Private fields

        /// <summary>
        /// The Icon's pointer/handle
        /// </summary>
        private IntPtr _iconPointer;

        /// <summary>
        /// A HWND to the calling window
        /// </summary>
        private IntPtr _mainWindowHandle;

        /// <summary>
        /// A "factory" function used to send data to the TrayIcon
        /// </summary>
        private Func<SystemTrayIconData[]> _iconDataFactory;

        /// <summary>
        /// Path to the icon's file
        /// </summary>
        private string _iconPath;

        #endregion


        public SystemTrayIcon(Func<SystemTrayIconData[]> iconDataFactory)
        {
            if (iconDataFactory is null)
                throw new ArgumentNullException(nameof(iconDataFactory));

            _iconDataFactory = iconDataFactory;
        }


        /// <summary>
        /// *Creates* the icon and gives it some data
        /// </summary>
        /// <param name="systemTrayIconData"></param>
        public void BuildIcon(string iconPath, IntPtr mainWindowHandle)
        {
            _iconPath = iconPath;
            _mainWindowHandle = mainWindowHandle;

            var trayIconData = _iconDataFactory();

            // Call C++ dll and create the icon. 
            _iconPointer = CoreDLL.CreateSystemTrayIcon(mainWindowHandle, iconPath, trayIconData, trayIconData.Length);
        }

        /// <summary>
        /// Allows "refreshing" the icon's data after adding a new project
        /// </summary>
        /// <param name="showAfterBuild"></param>
        public void RebuildIcon(bool showAfterBuild = false)
        {
            // Remove the icon 
            RemoveIcon();

            // Re-build the icon
            BuildIcon(_iconPath, _mainWindowHandle);


            // Show the icon if requested
            if (showAfterBuild == true)
                ShowIcon();
        }

        /// <summary>
        /// Shows the icon after it was created
        /// </summary>
        public void ShowIcon()
        {
            CoreDLL.ShowSystemTrayIcon(_iconPointer);
        }

        /// <summary>
        /// Removes the icon from System Tray. Should be called when app closes
        /// </summary>
        public void RemoveIcon()
        {
            CoreDLL.RemoveSystemTrayIcon(_iconPointer);
        }

    };
};