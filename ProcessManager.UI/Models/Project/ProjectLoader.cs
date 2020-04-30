namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;


    /// <summary>
    /// An implementation of <see cref="IProjectLoader"/>
    /// </summary>
    public class ProjectLoader : IProjectLoader
    {
        /// <summary>
        /// A process loader
        /// </summary>
        private readonly IProcessLoader _processLoader;
        private readonly ISerializer _serializer;


        /// <summary>
        /// The name of the file contaning the projects
        /// </summary>
        private readonly string _projectsFilename;
        
        /// <summary>
        /// The name of the Project config file
        /// </summary>
        private readonly string _projectConfigFilename;


        /// <summary>
        /// An enumerable of loaded projects
        /// </summary>
        private IEnumerable<Project> _projects;


        public ProjectLoader(IProcessLoader processLoader, ISerializer serializer, string projectsFilename, string projectConfigFilename)
        {
            _processLoader = processLoader;
            _serializer = serializer;

            _projectsFilename = projectsFilename;
            _projectConfigFilename = projectConfigFilename;
        }

        public void LoadProjectsDirectories()
        {
            try
            {
                // Using a json serializer, deserialzie the data inside the Projects file into C# objects
                _projects = _serializer.DeserializerProjects(File.ReadAllBytes(_projectsFilename));
            }
            // Projects file contains invalid json
            catch (JsonException jsonException)
            {
                throw new Exception($"Failed to read {_projectsFilename}, File doesn't contain valid json data. \nError: {jsonException}");
            }
            // An unknown error has occured
            catch (Exception exception)
            {
                throw new Exception($"Failed to read {_projectsFilename}. Uknown error has occured. \nError: {exception}");
            };
        }

        public void ValidateLoadedProjects()
        {
            foreach (var project in _projects)
            {
                // Check if project directory exists
                if (Directory.Exists(project.ProjectPath) == false)
                {
                    throw new Exception($"{project.ProjectPath} is not a valid directory or it doesn't exist");
                };

                // Check if project contains project config file
                if (File.Exists(Path.Combine(project.ProjectPath, _projectConfigFilename)) == false)
                {
                    throw new Exception($"{project.ProjectPath} doesn't contain a \"ProcessManger.Config.Json\" file");
                };
            };
        }


        public void LoadProjectProcesses()
        {
            foreach (var project in _projects)
            {
                // Load the process list inside the project
                project.ProcessList = new List<IProcessModel>(
                    // Read Project config file and deserialize
                    _processLoader.GetProcessListFromFile(project.ProjectPathWithConfig));
            };
        }


        public IEnumerable<Project> GetProjectsList()
        {
            return _projects;
        }

    };
};