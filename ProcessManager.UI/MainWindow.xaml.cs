namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const string DLL = "ProcessManager.Core.dll";

        private class ProcessModel
        {
            public string ProcessName { get; set; }
            public string ProcessArgs { get; set; }
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct sProcessModel
        {
            public string ProcessName { get; set; }
            public string ProcessArgs { get; set; }
        }


        List<ProcessModel> _processList;


        [DllImport(DLL)]
        private static extern void Initialize();


        [DllImport(DLL, CharSet = CharSet.Unicode)]
        private static extern void RunProcess(string processName, string processArgs);


        [DllImport(DLL, CharSet = CharSet.Unicode)]
        private static extern void CloseProcess(string processName);




        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct sStruct
        {
            public string Word { get; set; }
            public string Word2 { get; set; }

            //public sStruct(string word, string word2)
            //{
            //    Word = word;
            //    Word2 = word2;
            //}
        };


        [DllImport(DLL, CharSet = CharSet.Unicode)]
        private static extern void Test(sStruct[] s, int size);//sProcessModel[] processes, int arrayLength);


        public MainWindow()
        {
            InitializeComponent();

            // Setup process list
            _processList = GetProcessesFromFile();

            // Display processes
            _processList.ForEach(process => AddProcessToList(process));

            Initialize();


        }


        private unsafe void Button_Run_Processes_Click(object sender, RoutedEventArgs e)
        {
            sStruct[] s = new sStruct[]
            {
                new sStruct()
                {
                    Word = "b",
                    Word2 = "fdsa",
                },
                new sStruct()
                {
                    Word = "f",
                    Word2 = "fdsa",
                },
                new sStruct()
                {
                    Word = "r",
                    Word2 = "asdf",
                },
            };

            Test(s, s.Length);

            //var s = _processList.Select(process =>
            //new sProcessModel()
            //{
            //    ProcessName = process.ProcessName,
            //    ProcessArgs = process.ProcessArgs,
            //})
            //.ToArray();

            //Test(s, s.Length);
            //RunProcess(_processList[1].ProcessName, _processList[1].ProcessArgs);
        }

        private void Button_Close_Processes_Click(object sender, RoutedEventArgs e)
        {
            CloseProcess(_processList[1].ProcessName);
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

        private void AddProcessToList(ProcessModel process)
        {
            ProcessList.Children.Add(new TextBlock()
            {
                Text = process.ProcessName,
                TextAlignment = TextAlignment.Center,
            });
        }

    };
};