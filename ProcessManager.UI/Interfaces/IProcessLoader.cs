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
        /// <param name="filePath"> The path to the file </param>
        /// <returns></returns>
        public IEnumerable<ProcessModel> GetProcessesFromFile(string filePath);
    
    };
};
