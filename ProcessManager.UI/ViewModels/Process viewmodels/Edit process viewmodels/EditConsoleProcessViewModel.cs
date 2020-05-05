namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class EditConsoleProcessViewModel : EditProcessBaseViewModel
    {

        public static EditConsoleProcessViewModel DesignInstance => new EditConsoleProcessViewModel()
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



        private EditConsoleProcessViewModel() { }
        public EditConsoleProcessViewModel(ProjectItemViewModel projectViewModel, ProcessItemViewModel processItemViewModel)
        {
            var process = ValidateProcessType<ConsoleProcess>(processItemViewModel.Process);

            ProcessVM = processItemViewModel;
            ProjectVM = projectViewModel;

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
            var project = ProjectVM.Project;

            var index = project.ProcessList.IndexOf(ProcessVM.Process);

            if (index == -1)
                return;

            project.ProcessList[index] = new ConsoleProcess(ConsoleScript, ConsoleDirectory, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            };

            var projectBytes = DI.Serializer.SerializeProcessList(project.ProcessList);
              

            File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);

            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }
    };
};