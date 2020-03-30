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

        public string ProjectPathWithConfig => Path.Combine(ProjectPath, "ProcessManger.Config.Json");

        public IEnumerable<ProcessModel> ProcessList { get; set; }
    }
};
