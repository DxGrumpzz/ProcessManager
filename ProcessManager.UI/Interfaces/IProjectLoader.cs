namespace ProcessManager.UI
{
    using System.Collections.Generic;


    /// <summary>
    /// An interface describing a project loader
    /// </summary>
    public interface IProjectLoader
    {
        /// <summary>
        /// Loads the the projects directories
        /// </summary>
        public void LoadProjectsDirectories();

        /// <summary>
        /// Performs validation checks
        /// </summary>
        public void ValidateLoadedProjects();

        /// <summary>
        /// Loads the process data inside the projects config file
        /// </summary>
        public void LoadProjectProcesses();
        
        /// <summary>
        /// Returns an enumerable of <see cref="Project"/> 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Project> GetProjectsList();

    };
};
