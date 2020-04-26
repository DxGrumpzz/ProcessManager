namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;


    /// <summary>
    /// 
    /// </summary>

    public class Project
    {
        public string ProjectPath { get; set; }

        public string ProjectName => new DirectoryInfo(ProjectPath).Name;

        public string ProjectPathWithConfig => Path.Combine(ProjectPath, "ProcessManger.Config.Json");

        public IList<IProcessModel> ProcessList { get; set; }



        public void RunProject()
        {
            foreach (var process in ProcessList)
            {
                process.RunProcess();
            };
        }

        public void CloseProject()
        {
            foreach (var process in ProcessList)
            {
                process.CloseProcess();
            };
        }

        public void CloseProjectTree()
        {
            foreach (var process in ProcessList)
            {
                process.CloseProcess();
            };
        }

    };
};
