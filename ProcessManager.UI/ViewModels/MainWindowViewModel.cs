namespace ProcessManager.UI
{
    using System.Collections.Generic;
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

        
        /// <summary>
        /// The current list of processes as ProcessItemViewModel
        /// </summary>
        public IEnumerable<ProcessItemViewModel> Processes { get; private set; }


        public ICommand RunProcessesCommnad { get; }
        public ICommand CloseProcessesCommnad { get; }


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
            foreach(var processVM in Processes)
            {
                processVM.Process.RunProcess();
            };
        }

        private void ExecuteCloseProcessesCommnad()
        {
            // Close every process
            foreach(var processVM in Processes)
            {
                processVM.Process.CloseProcess();
            };
        }
    };
};
