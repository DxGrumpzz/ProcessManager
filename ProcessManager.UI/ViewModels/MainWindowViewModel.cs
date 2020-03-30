namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    /// The ViewModel for MainWindow
    /// </summary>
    public class MainWindowViewModel
    {
        public static MainWindowViewModel DesignInstance => new MainWindowViewModel(new ProcessModel[]
        {
            new ProcessModel()
            {
                ProcessPath = @"C:\Programs\App.exe",
            },
            new ProcessModel()
            {
                ProcessPath = @"D:\Software\Process.exe",
            },
            new ProcessModel()
            {
                ProcessPath = @"A:\asdf\fdas\Prog.bat",
            },
        });

        public static MainWindowViewModel DesignInstance2 => new MainWindowViewModel(new Project[]
        {
           new Project()
           {
               ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
           },

           new Project()
           {
               ProjectPath = $@"D:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
           },

           new Project()
           {
               ProjectPath = $@"A:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
           },

        });


        /// <summary>
        /// The current list of processes as ProcessItemViewModel
        /// </summary>
        public IEnumerable<ProcessItemViewModel> Processes { get; private set; }

        public IEnumerable<ProjectItemViewModel> Projects { get; set; }


        public ICommand RunProcessesCommnad { get; }
        public ICommand CloseProcessesCommnad { get; }


        public MainWindowViewModel(IEnumerable<Project> projects)
        {
            Projects = projects.Select(project =>
            {
                return new ProjectItemViewModel(project);
            });
        }

        public MainWindowViewModel(IEnumerable<ProcessModel> processes)
        {
            // "Convert" the list of ProcessModels to ProcessItemViewModel
            Processes = processes.Select(process =>
            {
                return new ProcessItemViewModel(process);
            });

            RunProcessesCommnad = new RelayCommand(ExecuteRunProcessesCommnad);
            CloseProcessesCommnad = new RelayCommand(ExecuteCloseProcessesCommnad);
        }


        private void ExecuteRunProcessesCommnad()
        {
            // Run every process
            foreach (var processVM in Processes)
            {
                processVM.Process.RunProcess();
            };
        }

        private void ExecuteCloseProcessesCommnad()
        {
            // Close every process
            foreach (var processVM in Processes)
            {
                processVM.Process.CloseProcess();
            };
        }
    };
};
