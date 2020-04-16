namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// A windows "implementation" of an open file dialog
    /// </summary>
    public class WindowsFolderDialog : IFolderDialog
    {
        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern void OpenDirectoryDialog(ref IntPtr path);

        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern void DeallocPathPointer(IntPtr path);


        /// <summary>
        /// Calls the windows Open file dialog and returns a string contaning the path to the selected folder
        /// </summary>
        /// <returns></returns>
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
