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

            // Check if the processes file exists
            if (File.Exists("Processes.txt") == false)
            {
                MessageBox.Show("Unable to find Processes.txt");
                
                // Exit application if it isn't
                Environment.Exit(1);
                return;
            };

            // Setup DI stuff
            SetupDI();

            // Initialize ProcessManager functionality
            CoreDLL.Initialize();


            // Create the main window
            (Current.MainWindow = new MainWindow(
                new MainWindowViewModel(DI.ProcessList)))
                .Show();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            // Close the processes when app exists
            DI.ProcessList.ForEach(process =>
            {
                CoreDLL.CloseProcess(process.ProcessID);
            });
        }


        private static void SetupDI()
        {
            // Setup file process loader
            DI.ProcessLoader = new ProcessLoader("Processes.txt");

            // Setup process list
            DI.ProcessList = new List<ProcessModel>(DI.ProcessLoader.GetProcessesFromFile("Processes.txt"));
        }

    };
};
