namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;

    /// <summary>
    /// A viewmodel for the <see cref="ProjectHeaderView"/>
    /// </summary>
    public class ProjectHeaderViewModel
    {

        public static ProjectHeaderViewModel DesignInstance => new ProjectHeaderViewModel(
            new Project()
            {
                ProjectPath = @"C:\Projects\Secret Projects",
                ProcessList = new List<IProcessModel>(),
            });


        #region Public properties

        /// <summary>
        /// A <see cref="Project"/> that is associated with this viewmodel
        /// </summary>
        public Project Project { get; set; }

        #endregion


        #region Commands
        public ICommand CopyToClipTrayCommand { get; }
      
        public ICommand OpenProjectDirectoryCommand { get; }
      
        #endregion


        public ProjectHeaderViewModel(Project projectItemVM)
        {
            Project = projectItemVM;

            // Bind commands
            CopyToClipTrayCommand = new RelayCommand(() =>
            {
                Clipboard.SetText(Project.ProjectPath, TextDataFormat.UnicodeText);
            });

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(Project.ProjectPath));
        }



    };
};