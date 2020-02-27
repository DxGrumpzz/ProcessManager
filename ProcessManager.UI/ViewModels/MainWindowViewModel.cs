namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Linq;


    /// <summary>
    /// 
    /// </summary>
    public class MainWindowViewModel
    {
        public List<ProcessItemViewModel> Processes { get; }

        public MainWindowViewModel(IEnumerable<ProcessModel> processes)
        {
            Processes = new List<ProcessItemViewModel>(processes.Select(process =>
            new ProcessItemViewModel()
            { 
                Process = process,
            }));


        }
    };
};
