namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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


            // Setup process list
            DI.ProcessList = GetProcessesFromFile();

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

        private List<ProcessModel> GetProcessesFromFile(string filePath = "Processes.txt")
        {
            var lines = File.ReadAllLines(filePath)
                .Where(line => string.IsNullOrWhiteSpace(line) == false);

            List<ProcessModel> processList = new List<ProcessModel>();

            for (int a = 0; a < lines.Count(); a++)
            {
                string currentLine = lines.ElementAt(a).ToLower();

                if (currentLine == "[process]")
                {
                    processList.Add(new ProcessModel()
                    {
                        ProcessName = lines.ElementAt(++a),
                    });
                }
                else if (currentLine == "[args]")
                {
                    processList.Last().ProcessArgs = lines.ElementAt(++a);
                };
            };

            return processList;
        }

    };
};
