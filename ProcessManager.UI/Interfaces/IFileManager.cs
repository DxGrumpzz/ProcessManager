namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;

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

    };
};