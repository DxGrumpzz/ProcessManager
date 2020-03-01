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

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            DI.ProcessList.ForEach(process =>
            {
                if (process.IsRunning == true)
                {
                    CoreDLL.CloseProcess(process.ProcessID);
                    process.ProcessID = 0;
                };
            });
        }

    };
};