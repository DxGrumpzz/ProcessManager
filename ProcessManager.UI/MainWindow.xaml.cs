namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Controls;

    public static class ProcessExtensions
    {
        private static class Win32
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);

            [return: MarshalAs(UnmanagedType.Bool)]
            [DllImport("user32.dll", SetLastError = true)]
            public static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);


            public delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);
            [DllImport("user32.dll")]
            public static extern bool EnumThreadWindows(uint dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        }

        public static IntPtr SendMessage(this Process p, out IntPtr hwnd, UInt32 msg, IntPtr wParam, IntPtr lParam)
        {
            hwnd = p.WindowHandles().FirstOrDefault();

            if (hwnd != IntPtr.Zero)
                return Win32.SendMessage(hwnd, msg, wParam, lParam);
            else
                return IntPtr.Zero;
        }

        //Posts a message to the first enumerated window in the first enumerated thread with at least one window, and returns the handle of that window through the hwnd output parameter if such a window was enumerated.  If a window was enumerated, the return value is the return value of the PostMessage call, otherwise the return value is false.
        public static bool PostMessage(this Process p, out IntPtr hwnd, UInt32 msg, IntPtr wParam, IntPtr lParam)
        {
            hwnd = p.WindowHandles().FirstOrDefault();
            if (hwnd != IntPtr.Zero)
                return Win32.PostMessage(hwnd, msg, wParam, lParam);
            else
                return false;
        }

        public static IEnumerable<IntPtr> WindowHandles(this Process process)
        {
            var handles = new List<IntPtr>();

                foreach (ProcessThread thread in process.Threads)
                {
                    //process.WaitForInputIdle();

                    Win32.EnumThreadWindows((uint)thread.Id, (hWnd, lParam) =>
                    {
                        handles.Add(hWnd); 
                        return true;
                    },
                    IntPtr.Zero);
                }
            return handles;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private class ProcessModel
        {
            public string ProcessName { get; set; }
            public string ProcessArgs { get; set; }
        }


        List<ProcessModel> _processList;

        Process _process;

        public MainWindow()
        {
            InitializeComponent();

            // Setup process list
            _processList = GetProcessesFromFile();

            // Display processes
            _processList.ForEach(process => AddProcessToList(process));

                
            _process = Process.Start(new ProcessStartInfo()
            { 
                FileName = "ProcessManager.Core.exe",
                //CreateNoWindow = true,
                //WindowStyle = ProcessWindowStyle.Hidden,
            });

        }


        private unsafe void Button_Run_Processes_Click(object sender, RoutedEventArgs e)
        {
            // "Convert" the string to an unmanaged string in memory
            IntPtr handle  = Marshal.StringToHGlobalUni(_processList[1].ProcessName);

            IntPtr hwndMessageWasSentTo = IntPtr.Zero;

            // Send a message (500) to the created process with LPARAM as the handle to the string
            _process.SendMessage(out hwndMessageWasSentTo, 0x500, IntPtr.Zero, handle);
        }

        private void Button_Close_Processes_Click(object sender, RoutedEventArgs e)
        {
            //IntPtr hwndMessageWasSentTo = IntPtr.Zero;

            //IntPtr wParam = IntPtr.Zero;
            //IntPtr lParam = IntPtr.Zero;

            //_process.PostMessage(out hwndMessageWasSentTo, 0x500, wParam, lParam);
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