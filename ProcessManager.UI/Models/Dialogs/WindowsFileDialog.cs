namespace ProcessManager.UI
{
    using Microsoft.Win32;

    /// <summary>
    /// A windows implementation of IFileDialog
    /// </summary>
    public class WindowsFileDialog : IFileDialog
    {
        /// <summary>
        /// A Win32 FileDialog
        /// </summary>
        OpenFileDialog _openFileDialog = new OpenFileDialog();

        /// <summary>
        /// The selected file pat
        /// </summary>
        public string SelectedFilePath => _openFileDialog.FileName;

        /// <summary>
        /// Opens a FileDialog. 
        /// Returns true if a file selection was made, 
        /// Returns false if user closed dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowOpenFileDialog()
        {
            // Show the dialog, return result 
            return _openFileDialog.ShowDialog().Value;
        }

    };
};