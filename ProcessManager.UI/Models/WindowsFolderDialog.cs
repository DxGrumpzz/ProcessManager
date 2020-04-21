namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;


    /// <summary>
    /// A windows "implementation" of an open file dialog
    /// </summary>
    public class WindowsFolderDialog : IFolderDialog
    {
        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern void OpenDirectoryDialog(ref IntPtr path);

        /// <summary>
        /// Calls the windows Open file dialog and returns a string contaning the path to the selected folder
        /// </summary>
        /// <returns></returns>
        public string ShowDialog()
        {
            IntPtr pathPointer = IntPtr.Zero;
            string pathString = null;

            try
            {
                OpenDirectoryDialog(ref pathPointer);

                pathString = Marshal.PtrToStringUni(pathPointer);
            }
            // This is a terrible practice.
            // Necessary to keep the app running IF on the off-chance that something happens in OpenDirectoryDialog function 
            catch { }
            finally
            {
                Marshal.FreeCoTaskMem(pathPointer);
            };

            return pathString;
        }

    };
};
