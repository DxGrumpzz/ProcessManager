namespace ProcessManager.UI
{
    using System.Collections.Generic;

    /// <summary>
    /// An interface for a file manager
    /// </summary>
    public interface IFileManager
    {
        /// <summary>
        /// Updates a project's config file 
        /// </summary>
        /// <param name="project"> The project to update </param>
        public void UpdateProjectConfig(Project project);

        /// <summary>
        /// Updates the app's projects list
        /// </summary>
        /// <param name="projects"> The projects list to save </param>
        public void UpdateProjectsList(IEnumerable<Project> projects);

    };
};