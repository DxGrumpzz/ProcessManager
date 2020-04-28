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
        private static extern bool OpenDirectoryDialog(ref IntPtr path);

        public string SelectedPath { get; private set; }


        /// <summary>
        /// Calls the windows Open file dialog and returns a string contaning the path to the selected folder
        /// </summary>
        /// <returns></returns>
        public bool ShowDialog()
        {
            IntPtr pathPointer = IntPtr.Zero;

            try
            {
                bool result = OpenDirectoryDialog(ref pathPointer);

                if (result == true)
                    SelectedPath = Marshal.PtrToStringUni(pathPointer);

                return result;
            }
            // This is a terrible practice.
            // Necessary to keep the app running IF on the off-chance something happens in OpenDirectoryDialog function 
            catch { return false; }
            finally
            {
                Marshal.FreeCoTaskMem(pathPointer);
            };
        }
    };
};
