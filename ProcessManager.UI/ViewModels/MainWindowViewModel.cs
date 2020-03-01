namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class MainWindowViewModel
    {
        public List<ProcessItemViewModel> Processes { get; }


        public ICommand RunProcessesCommnad { get; }
        public ICommand CloseProcessesCommnad { get; }
 
        public MainWindowViewModel(IEnumerable<ProcessModel> processes)
        {
            Processes = new List<ProcessItemViewModel>(processes.Select(process =>
            new ProcessItemViewModel()
            {
                Process = process,
            }));

            RunProcessesCommnad = new RelayCommand(ExecuteRunProcessesCommnad);
            CloseProcessesCommnad = new RelayCommand(ExecuteCloseProcessesCommnad);
        }


        private void ExecuteRunProcessesCommnad()
        {
            Processes.ForEach(processVM =>
            {
                ProcessModel process = processVM.Process;

                if (process.RunProcess() == true)
                    processVM.ProcessRunning = true;
            });
        }

        private void ExecuteCloseProcessesCommnad()
        {
            Processes.ForEach(processVM =>
            {
                ProcessModel process = processVM.Process;

                if (process.CloseProcess() == true)
                    processVM.ProcessRunning = false;
            });
        }
    };
};
