namespace ProcessManager.UI
{
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
    };
};