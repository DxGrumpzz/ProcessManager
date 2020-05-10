namespace ProcessManager.UI
{
    using System.IO;


    /// <summary>
    /// An interface used to "communicate" with the OS to get info about a directory
    /// </summary>
    public interface IFolderDialog
    {

        /// <summary>
        /// The user's selected path
        /// </summary>
        public string SelectedPath { get; }

        /// <summary>
        /// A string specifing an error that has occured
        /// </summary>
        public string ErrorString { get; }

        /// <summary>
        /// A boolean flag that indicates if an error has occured
        /// </summary>
        public bool HasError { get; }

        /// <summary>
        /// A boolean flag that indicates that the user has closed the dialog without choosing a path
        /// </summary>
        public bool DialogClosedWithoutPath { get; }



        /// <summary>
        /// Call the OS' folder dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowDialog();

        /// <summary>
        /// Call the OS' folder dialog and opens it it from a given directory path
        /// </summary>
        /// <returns></returns>
        public bool ShowDialogFrom(string directoryPath);


        /// <summary>
        /// Opens a folder 
        /// </summary>
        /// <param name="projectPath"> A directory path to open </param>
        bool OpenFolder(string projectPath);

    };
};