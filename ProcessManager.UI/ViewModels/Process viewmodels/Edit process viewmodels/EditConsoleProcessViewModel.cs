namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class EditConsoleProcessViewModel : EditProcessBaseViewModel
    {
        public static EditConsoleProcessViewModel DesignInstance => new EditConsoleProcessViewModel(
            new ProjectItemViewModel(new Project()
            {
                ProjectPath = @"C:\Development\npm test",
            }),
            new ProcessItemViewModel(new ProjectItemViewModel(
                new Project()
                {
                    ProjectPath = @"C:\Development\npm test",
                }),
                new ConsoleProcess("npm run start", @"C:\Development\npm test")))
        {
            
        };



        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _consoleDirectory;

        #endregion


        #region Public properties

        /// <summary>
        /// The new path to where the console will run from
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

        public string ProcessLabel { get; set; }

        public string ConsoleScript { get; set; }

        public bool ProcessVisibleOnStartup { get; set; }

        #endregion


        #region Commands

        public ICommand SelectConsoleDirectoryCommand { get; }

        #endregion



        public EditConsoleProcessViewModel(ProjectItemViewModel projectViewModel, ProcessItemViewModel processItemViewModel)
        {
            var process = ValidateProcessType<ConsoleProcess>(processItemViewModel.Process);

            ProcessVM = processItemViewModel;
            Project = projectViewModel;

            ProcessLabel = process.ProcessLabel;
            ProcessVisibleOnStartup = process.VisibleOnStartup;

            ConsoleDirectory = process.StartupDirectory;
            ConsoleScript = process.ConsoleScript;

            SelectConsoleDirectoryCommand = new RelayCommand(ExecuteSelectConsoleDirectoryCommand);
        }



        private void ExecuteSelectConsoleDirectoryCommand()
        {
            var folderDialog = DI.FolderDialog;

            if (folderDialog.ShowDialog() == true)
                ConsoleDirectory = folderDialog.SelectedPath;
        }

        protected override void ExecuteSaveProcessCommand()
        {
            var project = Project.Project;

            var index = project.ProcessList.IndexOf(ProcessVM.Process);

            if (index == -1)
                return;

            project.ProcessList[index] = new ConsoleProcess(ConsoleScript, ConsoleDirectory, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            };

            var projectBytes = DI.Serializer.SerializeProcessList(project.ProcessList);
              

            File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);

            DI.UI.ChangeView(View.ProjectItemView, Project);
        }
    };
};