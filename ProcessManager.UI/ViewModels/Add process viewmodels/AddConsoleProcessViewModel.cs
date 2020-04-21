namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class AddConsoleProcessViewModel : BaseViewModel
    {

        public static AddConsoleProcessViewModel DesignInstance => new AddConsoleProcessViewModel()
        {

        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _consoleDirectory;

        #endregion


        #region Public properties


        /// <summary>
        /// A ProjectViewModel which is used when the user wants to go back to the Project view and not lose any saved data
        /// </summary>
        public ProjectItemViewModel ProjectVM { get; }


        public string ConsoleDirectory
        {
            get => _consoleDirectory;
            set
            {
                _consoleDirectory = value;
                OnPropertyChanged();
            }
        }

        public string ConsoleScript { get; set; }

        public string ProcessLabel { get; set; }

        public bool ProcessVisibleOnStartup { get; set; }

        #endregion


        #region Commands

        public ICommand SelectDirectoryCommand { get; }

        public ICommand BackToMainPageCommand { get; }
        public ICommand BackToProjectPageCommand { get; }

        public ICommand AddProcessCommand { get; }


        #endregion



        private AddConsoleProcessViewModel() { }
        public AddConsoleProcessViewModel(ProjectItemViewModel projectVM)
        {
            ProjectVM = projectVM;

            SelectDirectoryCommand = new RelayCommand(ExecuteSelectDirectoryCommand);


            BackToMainPageCommand = new RelayCommand(ExecuteBackToMainPageCommand);
            BackToProjectPageCommand = new RelayCommand(ExecuteBackToProjectPageCommand);

            AddProcessCommand = new RelayCommand(ExecuteAddProcessCommand);
        }


        private void ExecuteSelectDirectoryCommand()
        {
            var folderDialog = DI.FolderDialog;

            string directory = folderDialog.ShowDialog();

            if (string.IsNullOrEmpty(directory))
                return;

            ConsoleDirectory = directory;
        }


        private void ExecuteBackToMainPageCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

        private void ExecuteBackToProjectPageCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }


        private void ExecuteAddProcessCommand()
        {
            // Validate directory path
            if (string.IsNullOrWhiteSpace(ConsoleDirectory) == true &&
                Directory.Exists(ConsoleDirectory) == false)
                return;

            // Validate console script
            if (string.IsNullOrWhiteSpace(ConsoleScript) == true)
                return;

            var project = ProjectVM.Project;

            ProcessModel process = new ProcessModel()
            {
                RunAsConsole = true,
                
                ProcessPath = ConsoleDirectory,

                StartInDirectory = ConsoleDirectory,
                ConsoleScript  = ConsoleScript,
                
                VisibleOnStartup = ProcessVisibleOnStartup,

                ProcessLabel = ProcessLabel
            };

            project.ProcessList.Add(process);

            // Convert the process list inside the project to json
            string jsonString = JsonSerializer.Serialize(
                project.ProcessList
                .Select(process => new
                {
                    RunAsConsole = process.RunAsConsole,

                    StartInDirectory = process.StartInDirectory,
                    ConsoleScript = process.ConsoleScript,

                    ProcessPath = process.ProcessPath,
                    ProcessArgs = process.ProcessArgs,
                    ProcessLabel = process.ProcessLabel,

                    VisibleOnStartup = process.VisibleOnStartup,
                }),
                new JsonSerializerOptions()
                {
                    WriteIndented = true,
                });


            // Write the json to the project's config file
            File.WriteAllText(project.ProjectPathWithConfig, jsonString);


            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }

    };
};