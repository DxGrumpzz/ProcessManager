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
        /// <summary>
        /// The current list of processes as ProcessItemViewModel
        /// </summary>
        public List<ProcessItemViewModel> Processes { get; }


        public ICommand RunProcessesCommnad { get; }
        public ICommand CloseProcessesCommnad { get; }


        public MainWindowViewModel(IEnumerable<ProcessModel> processes)
        {
            // "Convert" the list of ProcessModels to ProcessItemViewModel
            Processes = new List<ProcessItemViewModel>(processes.Select(process =>
            {
                return new ProcessItemViewModel(process);
            }));

            RunProcessesCommnad = new RelayCommand(ExecuteRunProcessesCommnad);
            CloseProcessesCommnad = new RelayCommand(ExecuteCloseProcessesCommnad);
        }


        private void ExecuteRunProcessesCommnad()
        {
            // Run every process
            Processes.ForEach(processVM =>
            {
                ProcessModel process = processVM.Process;

                if (process.RunProcess() == true)
                    processVM.ProcessRunning = true;
            });
        }

        private void ExecuteCloseProcessesCommnad()
        {
            // Close every process
            Processes.ForEach(processVM =>
            {
                ProcessModel process = processVM.Process;

                if (process.CloseProcess() == true)
                    processVM.ProcessRunning = false;
            });
        }
    };
};
