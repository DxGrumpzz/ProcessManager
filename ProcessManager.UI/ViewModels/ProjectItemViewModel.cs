namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectItemViewModel
    {
        public static ProjectItemViewModel DesignInstance => new ProjectItemViewModel(new Project()
        {
            ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
        });


        public Project Project { get; }

        public IEnumerable<ProcessItemViewModel> ProcessList { get; }
        

        public ICommand GotoProjectViewCommnad { get; }
        public ICommand GotoMainPageCommnad { get; }
        public ICommand CloseProjectCommand { get; }
        public ICommand RunProjectCommand { get; }



        public ProjectItemViewModel(Project project)
        {
            Project = project;

            ProcessList = Project.ProcessList
            .Select(process => new ProcessItemViewModel(process));

            GotoProjectViewCommnad = new RelayCommand(ExecuteGotoProjectViewCommnad);
            GotoMainPageCommnad = new RelayCommand(ExecuteGotoMainPageommnad);

            CloseProjectCommand = new RelayCommand(ExecuteCloseProjectCommand);
            RunProjectCommand = new RelayCommand(ExecuteRunProjectCommand);
        }

        private void ExecuteRunProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                process.RunProcess();
            };
        }

        private void ExecuteCloseProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                process.CloseProcess();
            };
        }

        private void ExecuteGotoMainPageommnad()
        {
            DI.MainWindowViewModel.CurrentView = new MainView(DI.MainWindowViewModel);
        }

        private void ExecuteGotoProjectViewCommnad()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(this);
        }
    };
};
