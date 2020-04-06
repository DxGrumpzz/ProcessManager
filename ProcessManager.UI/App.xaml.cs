namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    public delegate void CallbackFunc(IntPtr Data);


    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct SystemTrayIconData
    {
        public string ProjectName { get; set; }

        public IntPtr Data { get; set; }

        public CallbackFunc CallBack { get; set; }
    };


    public class SystemTrayIcon
    {
        private readonly string _icon;
        private readonly IntPtr _mainWindowHandle;
        private IntPtr _iconPointer;

        public SystemTrayIcon(string icon, IntPtr mainWindowHandle)
        {
            _icon = icon;
            _mainWindowHandle = mainWindowHandle;
        }

        public void CreateIcon(SystemTrayIconData[] systemTrayIconData)
        {
            _iconPointer = CoreDLL.CreateSystemTrayIcon(_mainWindowHandle, _icon, systemTrayIconData, systemTrayIconData.Length);
        }

        public void ShowIcon()
        {
            CoreDLL.ShowSystemTrayIcon(_iconPointer);
        }

        public void RemoveIcon()
        {
            CoreDLL.RemoveSystemTrayIcon(_iconPointer);
        }
    };

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private const string PROJECT_CONFIG_FILE_NAME = "ProcessManger.Config.Json";
        private const string PROJCES_FILE_NAME = "Projects.json";

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);


            // Check if the processes file exists
            if (File.Exists(PROJCES_FILE_NAME) == false)
            {
                MessageBox.Show($"Unable to find {PROJCES_FILE_NAME}");

                // Exit application
                Environment.Exit(1);
                return;
            };

            // Setup DI stuff
            SetupDI();

            DI.MainWindowViewModel = new MainWindowViewModel(DI.Projects);
            (Current.MainWindow = new MainWindow(DI.MainWindowViewModel))
            .Show();


            // This is here and not in SetupDI because SystemTrayIcon takes an HWND as a parameter so we need a valid window to initialize this class
            // Create the SystemTrayIcon class
            DI.SystemTrayIcon = new SystemTrayIcon("Resources\\Icon.ico", new WindowInteropHelper(Current.MainWindow).Handle);


            DI.SystemTrayIcon.CreateIcon(DI.Projects
                .Select(project =>
                {
                    GCHandle dataHandle = GCHandle.Alloc(project);

                    var s = new SystemTrayIconData()
                    {
                        ProjectName = new DirectoryInfo(project.ProjectPath).Name,

                        CallBack = (data) =>
                        {
                            if(!(GCHandle.FromIntPtr(data).Target is Project project))
                            {
                                Debugger.Break();
                                return;
                            };

                            Debugger.Break();
                        },

                        Data = GCHandle.ToIntPtr(dataHandle),
                    };

                    return s;

                }).ToArray());

            DI.SystemTrayIcon.ShowIcon();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);

            DI.SystemTrayIcon.RemoveIcon();

            // Close the processes when app exists
            foreach (var project in DI.Projects)
            {
                foreach (var process in project.ProcessList)
                {
                    process.CloseProcessTree();
                };
            };
        }


        private static void SetupDI()
        {
            // Create a process loader
            DI.ProcessLoader = new JsonProcessLoader();

            // Setup project loader
            DI.ProjectLoader = new ProjectLoader(
                processLoader: DI.ProcessLoader,
                filename: PROJCES_FILE_NAME,
                projectConfigFilename: PROJECT_CONFIG_FILE_NAME);

            // Load the projects
            DI.ProjectLoader.LoadProjectsDirectories();
            DI.ProjectLoader.ValidateLoadedProjects();
            DI.ProjectLoader.LoadProjectProcesses();


            // Load the projects list into DI
            DI.Projects = DI.ProjectLoader.GetProjectsList();

        }

    };
};
