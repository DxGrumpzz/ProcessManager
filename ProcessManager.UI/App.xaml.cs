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
            if (File.Exists("ProcessList.json") == false)
            {
                MessageBox.Show("Unable to find ProcessList.json");
                
                // Exit application if it isn't
                Environment.Exit(1);
                return;
            };

            // Setup DI stuff
            SetupDI();

            // Initialize ProcessManager functionality
            // CoreDLL.Initialize();


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
            DI.ProcessLoader = new JsonProcessLoader("ProcessList.json");

            // Check if process list file is valid
            var processList = DI.ProcessLoader.GetProcessListFromFile();
            if (processList == null)
            {
                MessageBox.Show("ProcessList.json contains invalid data", "Error");

                // Exit application if it isn't
                Environment.Exit(1);
                return;
            };

            // Setup process list
            DI.ProcessList = new List<ProcessModel>(processList);
        }

    };
};
