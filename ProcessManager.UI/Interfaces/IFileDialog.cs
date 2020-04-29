namespace ProcessManager.UI
{
    /// <summary>
    /// An interface for an OS file dialog
    /// </summary>
    public interface IFileDialog
    {
        /// <summary>
        /// The selected file pat
        /// </summary>
        public string SelectedFilePath { get; }

        /// <summary>
        /// Opens a FileDialog. 
        /// Returns true if a file selection was made, 
        /// Returns false if user closed dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowOpenFileDialog();

    };
};