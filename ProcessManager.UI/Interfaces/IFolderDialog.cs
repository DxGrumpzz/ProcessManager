namespace ProcessManager.UI
{
    using System.IO;


    /// <summary>
    /// An interface used to "communicate" with the OS to get info about a directory
    /// </summary>
    public interface IFolderDialog
    {
        
        public string SelectedPath { get; }

        /// <summary>
        /// Call the OS' folder dialog
        /// </summary>
        /// <returns></returns>
        public bool ShowDialog();

    };
};