namespace ProcessManager.UI
{
    using System.Collections.Generic;

    /// <summary>
    /// A process loader 
    /// </summary>
    public interface IProcessLoader
    {
        /// <summary>
        /// Retrieves an <see cref="IEnumerable{T}"/> of ProcessModel from a file
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ProcessModel> GetProcessesFromFile();
        
        /// <summary>
        /// Checks if the Processes file is valid
        /// </summary>
        /// <returns></returns>
        public bool IsProcessesFileFileValid();

    };
};
