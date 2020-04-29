namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;


    /// <summary>
    /// A class contaning info and function used to interact with a Project
    /// </summary>
    public class Project
    {
        /// <summary>
        /// A path to the project's directory
        /// </summary>
        public string ProjectPath { get; set; }

        /// <summary>
        /// The name of this project
        /// </summary>
        public string ProjectName => new DirectoryInfo(ProjectPath).Name;

        /// <summary>
        /// The path to the project's config file
        /// </summary>
        public string ProjectPathWithConfig => Path.Combine(ProjectPath, "ProcessManger.Config.Json");

        /// <summary>
        /// This projce's process list
        /// </summary>
        public IList<IProcessModel> ProcessList { get; set; }


        /// <summary>
        /// Run every process in this project
        /// </summary>
        public void RunProject()
        {
            foreach (var process in ProcessList)
            {
                process.RunProcess();
            };
        }

        /// <summary>
        /// Close every process in this project
        /// </summary>
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
