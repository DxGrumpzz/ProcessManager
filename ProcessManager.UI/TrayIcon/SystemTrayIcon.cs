namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// A class responsible for interaction with the TrayIcon
    /// </summary>
    public class SystemTrayIcon
    {
        /// <summary>
        /// The Icon's pointer/handle
        /// </summary>
        private IntPtr _iconPointer;

        private IntPtr _mainWindowHandle;

        private Func<SystemTrayIconData[]> _iconDataFactory;

        private string _iconPath;


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


        public void RebuildIcon(bool showAfterBuild = false)
        {
            RemoveIcon();
            BuildIcon(_iconPath, _mainWindowHandle);


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