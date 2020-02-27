namespace ProcessManager.UI
{
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProcessItemViewModel
    {

        public ProcessModel Process { get; set; }

        public string ProcessName => Process.ProcessName;


        public ICommand RunProcessCommand { get; }
        public ICommand CloseProcessCommand { get; }


        public ProcessItemViewModel()
        {
            RunProcessCommand = new RelayCommand(ExecuteRunProcessCommand);
            CloseProcessCommand= new RelayCommand(ExecuteCloseProcessCommand);
        }


        private void ExecuteRunProcessCommand()
        {
            
        }

        private void ExecuteCloseProcessCommand()
        {

        }

    };
};
