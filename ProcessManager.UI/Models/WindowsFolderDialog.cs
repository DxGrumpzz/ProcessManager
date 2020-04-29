namespace ProcessManager.UI
{
    using System;
    using System.Runtime.InteropServices;


    /// <summary>
    /// A windows "implementation" of an open file dialog
    /// </summary>
    public class WindowsFolderDialog : IFolderDialog
    {

        /// <summary>
        /// Calls a COM function to open the Windows FileDialog
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern bool OpenDirectoryDialog(ref IntPtr path);


        public string SelectedPath { get; private set; }


        /// <summary>
        /// Calls the windows Open file dialog and returns a string contaning the path to the selected folder
        /// </summary>
        /// <returns></returns>
        public bool ShowDialog()
        {
            // A pointer to an unmanaged string
            IntPtr pathPointer = IntPtr.Zero;

            try
            {
                // Open the folder dialog
                bool result = OpenDirectoryDialog(ref pathPointer);

                // If user chose a folder
                if (result == true)
                    // Marhsal the unmanaged to managed string
                    SelectedPath = Marshal.PtrToStringUni(pathPointer);

                // return the selected path
                return result;
            }
            // This is a terrible practice.
            // Necessary to keep the app running IF on the off-chance something happens in OpenDirectoryDialog function 
            catch { return false; }
            finally
            {
                // Because we call COM function and pathPointer is allocated inside a COM function we must free it using COM
                Marshal.FreeCoTaskMem(pathPointer);
            };
        }
    };
};
