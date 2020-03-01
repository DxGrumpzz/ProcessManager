namespace ProcessManager.UI
{
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessItemViewModel : BaseViewModel
    {
        private bool _processRunning;

        public ProcessModel Process { get; set; }


        public string ProcessName => Process.ProcessName;
        public bool ProcessRunning
        {
            get => _processRunning;
            set 
            { 
                _processRunning = value;
                OnPropertyChanged();
            }
        }


        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ProcessItemViewModel()
        {
            RunProcessCommand = new RelayCommand(ExecuteRunProcessCommand);
            CloseProcessCommand= new RelayCommand(ExecuteCloseProcessCommand);
        }


        private void ExecuteRunProcessCommand()
        {
            if (Process.IsRunning == false)
            {
                ulong processID = CoreDLL.RunProcess(Process.ProcessName, Process.ProcessArgs);
                Process.ProcessID = processID;

                ProcessRunning = processID != 0 ? true : false;
            };
        }

        private void ExecuteCloseProcessCommand()
        {
            if (Process.IsRunning == true)
            {
                CoreDLL.CloseProcess(Process.ProcessID);
                Process.ProcessID = 0;

                ProcessRunning = false;
            };
        }

    };
};
