namespace ProcessManager.UI
{
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
            
            ProcessList = new ProcessModel[]
            { 
                new ProcessModel()
                {
                    ProcessPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.exe",
                },

                new ProcessModel()
                {
                    ProcessPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.bat",
                    ProcessID = 1,
                },

                new ProcessModel()
                {
                    ProcessPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}.exe",
                    ProcessID = 1,
                    ProcessVisibilityState = ProcessVisibilityState.Hidden
                },
            },
        });

        /// <summary>
        /// The project associated with this viewmodel
        /// </summary>
        public Project Project { get; }

        /// <summary>
        /// The project's processes as a "list" of ProcessItemViewModel
        /// </summary>
        public IEnumerable<ProcessItemViewModel> ProcessList { get; }
        

        public ICommand GotoProjectViewCommnad { get; }
        public ICommand GotoMainPageCommnad { get; }
        public ICommand CloseProjectCommand { get; }
        public ICommand RunProjectCommand { get; }



        public ProjectItemViewModel(Project project)
        {
            Project = project;

            // Convert the list of processes inside the project to a list of ProcessItemViewModel
            ProcessList = Project.ProcessList
            .Select(process => new ProcessItemViewModel(process)).ToArray();


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
                process.CloseProcessTree();
            };
        }


        private void ExecuteGotoMainPageommnad()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

        private void ExecuteGotoProjectViewCommnad()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(this);
        }
    };
};
