namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectListItemViewModel
    {
        public static ProjectListItemViewModel DesignInstance => new ProjectListItemViewModel(
            new Project()
            {
                ProjectPath = @"C:\Software\Secret Project",
            })
        {
        };


        public Project Project { get; }



        #region Public commands


        public ICommand RunProjectCommand { get; }
        public ICommand CloseProjectCommand { get; }

        public ICommand SwitchToProjectViewCommand { get; }

        #endregion



        private ProjectListItemViewModel() { }
        public ProjectListItemViewModel(Project project)
        {
            Project = project;

            RunProjectCommand = new RelayCommand(ExecuteRunProjectCommand, singleFire: true);
            CloseProjectCommand = new RelayCommand(ExecuteCloseProjectCommand, singleFire: true);

            SwitchToProjectViewCommand = new RelayCommand(ExecuteSwitchToProjectViewCommand);
        }


        private void ExecuteRunProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                Task.Run(process.RunProcess);
            };
        }

        private void ExecuteCloseProjectCommand()
        {
            // Only close running processes 
            var runningProcesses = Project.ProcessList.Where(process => process.IsRunning == true);

            foreach (var process in runningProcesses)
            {
                Task.Run(process.CloseProcess);
            };
        }

        private void ExecuteSwitchToProjectViewCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(new ProjectItemViewModel(Project));
        }

    };
};