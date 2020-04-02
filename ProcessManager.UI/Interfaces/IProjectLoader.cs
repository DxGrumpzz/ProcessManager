namespace ProcessManager.UI
{
    using System.Collections.Generic;


    /// <summary>
    /// 
    /// </summary>
    public interface IProjectLoader
    {
        void LoadProjectsDirectories();

        public void ValidateLoadedProjects();

        public void LoadProjectProcesses();
        
        IEnumerable<Project> GetProjectsList();
    };
};
