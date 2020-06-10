namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// An implementation of the <see cref="IFileManager"/> iterface
    /// </summary>
    public class FileManager : IFileManager
    {
        private readonly ISerializer _serializer;

        public FileManager(ISerializer serializer)
        {
            _serializer = serializer;
        }

        /// <summary>
        /// Updates a project's config file 
        /// </summary>
        /// <param name="project"> The project to update </param>
        public void UpdateProjectConfig(Project project)
        {
            // Serialize project list 
            var projectBytes = _serializer.SerializeProcessList(project.ProcessList);

            // Update project config file
            File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);
        }

        /// <summary>
        /// Updates the app's projects list
        /// </summary>
        /// <param name="projects"> The projects list to save </param>
        public void UpdateProjectsList(IEnumerable<Project> projects)
        {
            // Convert the json object to json string
            var jsonBytes = _serializer.SerializerProjects(DI.Projects);

            // Write the json string to Projects file
            File.WriteAllBytes(Localization.PROJECTS_FILE_NAME, jsonBytes);
        }
    };
};