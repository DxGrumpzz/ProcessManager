namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            if (File.Exists("Processes.txt") == false)
            {
                MessageBox.Show("Unable to find Processes.txt");
                Environment.Exit(1);
                return;
            };

            SetupDI();

            CoreDLL.Initialize();

            (Current.MainWindow = new MainWindow(
                new MainWindowViewModel(DI.ProcessList)))
                .Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DI.ProcessList.ForEach(process =>
            {
                CoreDLL.CloseProcess(process.ProcessID);
            });
        }


        private static void SetupDI()
        {
            // Setup file process loader
            DI.ProcessLoader = new ProcessLoader();

            // Setup process list
            DI.ProcessList = new List<ProcessModel>(DI.ProcessLoader.GetProcessesFromFile("Processes.txt"));
        }

    };
};
