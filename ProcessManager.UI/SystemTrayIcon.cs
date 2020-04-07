namespace ProcessManager.UI
{
    using System;
    using System.Linq;

    public class SystemTrayIcon
    {
        private readonly string _icon;
        private readonly IntPtr _mainWindowHandle;
        private IntPtr _iconPointer;

        public SystemTrayIcon(string icon, IntPtr mainWindowHandle)
        {
            _icon = icon;
            _mainWindowHandle = mainWindowHandle;
        }

        public void CreateIcon(SystemTrayIconData[] systemTrayIconData)
        {
            _iconPointer = CoreDLL.CreateSystemTrayIcon(_mainWindowHandle, _icon, systemTrayIconData, systemTrayIconData.Length);
        }

        public void ShowIcon()
        {
            CoreDLL.ShowSystemTrayIcon(_iconPointer);
        }

        public void RemoveIcon()
        {
            CoreDLL.RemoveSystemTrayIcon(_iconPointer);
        }
    };
};
