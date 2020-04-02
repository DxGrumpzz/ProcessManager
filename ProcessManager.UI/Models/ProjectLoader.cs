namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.Json;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectLoader : IProjectLoader
    {
        private readonly IProcessLoader _processLoader;

        private readonly string _filename;
        private readonly string _projectConfigFilename;

        private IEnumerable<Project> _projects;

        public ProjectLoader(IProcessLoader processLoader, string filename, string projectConfigFilename)
        {
            _processLoader = processLoader;
            _filename = filename;
            _projectConfigFilename = projectConfigFilename;
        }


        public bool ProjectsFileExists()
        {
            if (File.Exists(_filename) == false)
            {
                return false;
            };

            return true;
        }


        public void ValidateLoadedProjects()
        {
            foreach (var project in _projects)
            {
                if (Directory.Exists(project.ProjectPath) == false)
                {
                    throw new Exception($"{project.ProjectPath} is not a valid directory or it doesn't exist");
                };

                if (File.Exists(Path.Combine(project.ProjectPath, _projectConfigFilename)) == false)
                {
                    throw new Exception($"{project.ProjectPath} doesn't contain a \"ProcessManger.Config.Json\" file");
                };
            };
        }


        public void LoadProjectsDirectories()
        {
            try
            {
                _projects = JsonSerializer.Deserialize<IEnumerable<Project>>(File.ReadAllBytes(_filename));
            }
            catch (JsonException jsonException)
            {
                throw new Exception($"Failed to read {_filename}, File doesn't contain valid json data");
            }
            catch (Exception exception)
            {
                throw new Exception($"Failed to read {_filename}. Uknown error has occured");
            };
        }


        public void LoadProjectProcesses()
        {
            foreach (var project in _projects)
            {
                project.ProcessList = _processLoader.GetProcessListFromFile(project.ProjectPathWithConfig);
            };
        }


        public IEnumerable<Project> GetProjectsList()
        {
            return _projects;
        }
    };
};
