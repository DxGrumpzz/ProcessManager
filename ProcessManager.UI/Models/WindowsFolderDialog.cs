namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// 
    /// </summary>
    public class WindowsFolderDialog : IFolderDialog
    {
        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern void OpenDirectoryDialog(ref IntPtr path);

        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern void DeallocPathPointer(IntPtr path);

        public string ShowDialog()
        {
            IntPtr pathPointer = IntPtr.Zero;

            OpenDirectoryDialog(ref pathPointer);

            string pathString = Marshal.PtrToStringUni(pathPointer);

            DeallocPathPointer(pathPointer);

            return pathString;
        }

    };
};
