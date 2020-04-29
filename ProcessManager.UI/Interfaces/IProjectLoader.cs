namespace ProcessManager.UI
{
    using System.Collections.Generic;


    /// <summary>
    /// 
    /// </summary>
    public interface IProjectLoader
    {
        public void LoadProjectsDirectories();

        public void ValidateLoadedProjects();

        public void LoadProjectProcesses();
        
        public IEnumerable<Project> GetProjectsList();
    };
};
