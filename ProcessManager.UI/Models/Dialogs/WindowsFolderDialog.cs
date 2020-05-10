namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
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

        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern bool OpenDirectoryDialogFrom(ref IntPtr path, string openFrom, ref IntPtr stringErrorOut);

        /// <summary>
        /// Frees the stringErrorOut allocated in <see cref="OpenDirectoryDialogFrom(ref IntPtr, string, ref IntPtr)"/>
        /// </summary>
        /// <param name="stringErrorOut"></param>
        /// <returns></returns>
        [DllImport("ProcessManager.Core.dll", CharSet = CharSet.Unicode)]
        private static extern bool FreeOutErrorString(ref IntPtr stringErrorOut);
        

        public string SelectedPath { get; private set; }

        public string ErrorString { get; private set; }

        public bool HasError => !string.IsNullOrWhiteSpace(ErrorString);

        public bool DialogClosedWithoutPath => (HasError == false) && (string.IsNullOrWhiteSpace(SelectedPath) == true);


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


        /// <summary>
        /// Call the OS' folder dialog and opens it it from a given directory path
        /// </summary>
        /// <returns></returns>
        public bool ShowDialogFrom(string openFrom)
        {
            // A pointer to an unmanaged string
            IntPtr pathPointer = IntPtr.Zero;
            IntPtr outErrorStringPointer = IntPtr.Zero;


            try
            {
                // Open the folder dialog
                bool result = OpenDirectoryDialogFrom(ref pathPointer, openFrom, ref outErrorStringPointer);

                // If user chose a folder
                if (result == true)
                    // Marhsal the unmanaged to managed string
                    SelectedPath = Marshal.PtrToStringUni(pathPointer);
                else
                    ErrorString = Marshal.PtrToStringUni(outErrorStringPointer);

                // return the selected path
                return result;
            }
            // This is a terrible practice.
            // Necessary to keep the app running IF on the off-chance something happens in OpenDirectoryDialog function 
            catch
            {
                ErrorString = "An unexpected error has occured";
                return false;
            }
            finally
            {
                // Because we allocate a string from a COM function we must free it using COM
                Marshal.FreeCoTaskMem(pathPointer);
                FreeOutErrorString(ref outErrorStringPointer);
            };
        }

    };
};
