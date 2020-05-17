namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
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


        /// <summary>
        /// A path to a directory, which the console process will start on
        /// </summary>
        public string ConsoleDirectory
        {
            get => _consoleDirectory;
            set
            {
                _consoleDirectory = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A script to run in the console
        /// </summary>
        public string ConsoleScript { get; set; }

        /// <summary>
        /// A label associated with this process
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// A boolean flag indiacting if this process will be visible when it starts
        /// </summary>
        public bool ProcessVisibleOnStartup { get; set; } = true;

        #endregion


        #region Commands

        public ICommand SelectDirectoryCommand { get; }
        public ICommand SelectCurrentDirectoryCommand { get; }


        public ICommand BackToMainPageCommand { get; }
        public ICommand BackToProjectPageCommand { get; }


        public ICommand AddProcessCommand { get; }


        #endregion



        private AddConsoleProcessViewModel() { }
        public AddConsoleProcessViewModel(ProjectItemViewModel projectVM)
        {
            ProjectVM = projectVM;

            SelectDirectoryCommand = new RelayCommand(ExecuteSelectDirectoryCommand);
            SelectCurrentDirectoryCommand = new RelayCommand(() =>
                ConsoleDirectory = ProjectVM.Project.ProjectPath);

            BackToMainPageCommand = new RelayCommand(ExecuteBackToMainPageCommand);
            BackToProjectPageCommand = new RelayCommand(ExecuteBackToProjectPageCommand);

            AddProcessCommand = new RelayCommand(
                ExecuteAddProcessCommand,
                () => ConsoleDirectory?.Length >= 3);
        }


        private void ExecuteSelectDirectoryCommand()
        {
            var folderDialog = DI.FolderDialog;


            bool folderDialogResult;

            // Check if the Console directory actually exists
            if (string.IsNullOrWhiteSpace(ConsoleDirectory) == true &&
                // Check if directory exists
                Directory.Exists(ConsoleDirectory) == false)
            {
                // Show the folder dialog 
                folderDialogResult = folderDialog.ShowDialog();
            }
            else
                // Show the folder dialog from ConsoleDirectory
                folderDialogResult = folderDialog.ShowDialogFrom(ConsoleDirectory);


            // Check if the user has chose a valid directory
            if (folderDialogResult == false)
                return;

            // set the console directory to the selectd path
            ConsoleDirectory = folderDialog.SelectedPath;
        }


        private void ExecuteBackToMainPageCommand()
        {
            // Go back to to the projects list view
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

        private void ExecuteBackToProjectPageCommand()
        {
            // Go back to the project item view
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }


        private void ExecuteAddProcessCommand()
        {
            // Validate directory path
            if (string.IsNullOrWhiteSpace(ConsoleDirectory) == true &&
                Directory.Exists(ConsoleDirectory) == false)
            {
                DI.UserDialog.ShowDialog("Invalid directory was selected");
                return;
            };

            // Validate console script
            if (string.IsNullOrWhiteSpace(ConsoleScript) == true)
            {
                DI.UserDialog.ShowDialog("Console script shouldn't be empty");
                return;
            };

            var project = ProjectVM.Project;

            // Add the new process to the project's process list
            project.ProcessList.Add(new ConsoleProcess(ConsoleScript, ConsoleDirectory, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            });

            // Serialize the process list after adding the project
            string jsonString = SerializeProcessList(project);

            // Write the json to the project's config file
            File.WriteAllText(project.ProjectPathWithConfig, jsonString);


            ExecuteBackToProjectPageCommand();
        }

    };
};