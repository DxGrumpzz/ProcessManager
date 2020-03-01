namespace ProcessManager.UI
{
    using System.ComponentModel;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainWindowViewModel viewModel) :
            this()
        {
            DataContext = viewModel;
        }

        private void Button_Run_Processes_Click(object sender, RoutedEventArgs e)
        {
            DI.ProcessList.ForEach(process =>
            {
                var processID = CoreDLL.RunProcess(process.ProcessName, process.ProcessArgs);
                process.ProcessID = processID;
            });
        }

        private void Button_Close_Processes_Click(object sender, RoutedEventArgs e)
        {
            DI.ProcessList.ForEach(process =>
            {
                CoreDLL.CloseProcess(process.ProcessID);
            });
        }


        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DI.ProcessList.ForEach(process =>
            {
                CoreDLL.CloseProcess(process.ProcessID);
            });
        }


        private void RunProcessButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            ProcessModel process = (ProcessModel)button.DataContext;

            if (process.IsRunning == false)
            {
                ulong processID = CoreDLL.RunProcess(process.ProcessName, process.ProcessArgs);
                process.ProcessID = processID;
            };
        }

        private void CloseProcessButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            ProcessModel process = (ProcessModel)button.DataContext;


            if (process.IsRunning == true)
            {
                CoreDLL.CloseProcess(process.ProcessID);
                process.ProcessID = 0;
            };
        }

        
    };
};