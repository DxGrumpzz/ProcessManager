namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private class ProcessModel
        {
            public string ProcessName { get; set; }
            public string ProcessArgs { get; set; }

            public ulong ProcessID { get; set; }

            public bool IsRunning => ProcessID != 0;
        };


        private const string DLL = "ProcessManager.Core.dll";

        List<ProcessModel> _processList;


        [DllImport(DLL)]
        private static extern void Initialize();

        [DllImport(DLL, CharSet = CharSet.Unicode)]
        private static extern ulong RunProcess(string processName, string processArgs);

        [DllImport(DLL)]
        private static extern void CloseProcess(ulong processID);



        public MainWindow()
        {

            if (File.Exists("Processes.txt") == false)
            {
                MessageBox.Show("Unable to find Processes.txt");
                Environment.Exit(1);
                return;
            };


            InitializeComponent();

            // Setup process list
            _processList = GetProcessesFromFile();

            // Display processes
            _processList.ForEach(process => ProcessList.Items.Add(process));


            Initialize();
        }



        private void Button_Run_Processes_Click(object sender, RoutedEventArgs e)
        {
            _processList.ForEach(process =>
            {
                var processID = RunProcess(process.ProcessName, process.ProcessArgs);
                process.ProcessID = processID;
            });
        }

        private void Button_Close_Processes_Click(object sender, RoutedEventArgs e)
        {
            _processList.ForEach(process =>
            {
                CloseProcess(process.ProcessID);
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            _processList.ForEach(process =>
            {
                CloseProcess(process.ProcessID);
            });
        }

        private void RunProcessButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            ProcessModel process = (ProcessModel)button.DataContext;

            if (process.IsRunning == false)
            {
                ulong processID = RunProcess(process.ProcessName, process.ProcessArgs);
                process.ProcessID = processID;
            };
        }

        private void CloseProcessButtonClick(object sender, RoutedEventArgs e)
        {
            FrameworkElement button = (FrameworkElement)sender;
            ProcessModel process = (ProcessModel)button.DataContext;


            if (process.IsRunning == true)
            {
                CloseProcess(process.ProcessID);
                process.ProcessID = 0;
            };
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