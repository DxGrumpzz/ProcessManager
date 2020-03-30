namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectItemViewModel
    {

        public Project Project { get; }

        public ProjectItemViewModel(Project project)
        {
            Project = project;
        }
    };
};
