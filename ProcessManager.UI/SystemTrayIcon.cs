namespace ProcessManager.UI
{
    using System;

    /// <summary>
    /// A class responsible for interaction with the TrayIcon
    /// </summary>
    public class SystemTrayIcon
    {

        /// <summary>
        /// Path to the icon's bitmap
        /// </summary>
        private readonly string _iconPath;

        /// <summary>
        /// A handle to the main window
        /// </summary>
        private readonly IntPtr _mainWindowHandle;

        /// <summary>
        /// The Icon's pointer/handle
        /// </summary>
        private IntPtr _iconPointer;


        public SystemTrayIcon(string iconPath, IntPtr mainWindowHandle)
        {
            _iconPath = iconPath;
            _mainWindowHandle = mainWindowHandle;
        }

        /// <summary>
        /// *Creates* the icon and gives it some data
        /// </summary>
        /// <param name="systemTrayIconData"></param>
        public void CreateIcon(SystemTrayIconData[] systemTrayIconData)
        {
            // Call C++ dll and create the icon. 
            _iconPointer = CoreDLL.CreateSystemTrayIcon(_mainWindowHandle, _iconPath, systemTrayIconData, systemTrayIconData.Length);
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
