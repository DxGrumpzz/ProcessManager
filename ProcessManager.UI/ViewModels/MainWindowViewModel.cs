namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;
    using System.Windows.Input;


    /// <summary>
    /// The ViewModel for MainWindow
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        
        public static MainWindowViewModel DesignInstance => new MainWindowViewModel(new Project[]
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


        private ContentControl _currentView = new MainView();


        /// <summary>
        /// The current list of processes as ProcessItemViewModel
        /// </summary>
        public IEnumerable<ProcessItemViewModel> Processes { get; private set; }

        public IEnumerable<ProjectItemViewModel> Projects { get; set; }


        public ContentControl CurrentView 
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }



        public ICommand RunProcessesCommnad { get; }
        public ICommand CloseProcessesCommnad { get; }
        public ICommand GotoProjectViewCommnad { get; }


        public MainWindowViewModel(IEnumerable<Project> projects)
        {
            Projects = projects.Select(project =>
            {
                return new ProjectItemViewModel(project);
            });

            GotoProjectViewCommnad = new RelayCommand(ExecuteGotoProjectViewCommnad);
        }



        private void ExecuteGotoProjectViewCommnad()
        {
            
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
