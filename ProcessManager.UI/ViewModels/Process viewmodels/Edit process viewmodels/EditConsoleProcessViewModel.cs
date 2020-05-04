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
        private string _processLabel;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _consoleDirectory;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _consoleScript;

        #endregion


        #region Public properties

        public string ProcessLabel
        {
            get => _processLabel;
            set => _processLabel = value;
        }

        public string ConsoleDirectory
        {
            get => _consoleDirectory;
            set
            {
                _consoleDirectory = value;
                OnPropertyChanged();
            }
        }

        public string ConsoleScript
        {
            get => _consoleScript;
            set
            {
                _consoleScript = value;
                OnPropertyChanged();
            }
        }

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

            var projectBytes = DI.Serializer.Serialize(
                project.ProcessList
                .Select(process =>
                {
                    switch (process)
                    {
                        case ConsoleProcess consoleProcess:
                            return ConsoleProcessAsJsonProcess(consoleProcess);

                        case GUIProcess guiProcess:
                            return GUIProcessAsJsonProcess(guiProcess);

                        default:
                        {
                            Debugger.Break();
                            return default;
                        };
                    };
                }));


            File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);

            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }




        /// <summary>
        /// Converts a <see cref="GUIProcess"/> to a <see cref="ConsoleProcess"/> 
        /// </summary>
        /// <param name="consoleProcess"></param>
        /// <returns></returns>
        private JsonProcessModel ConsoleProcessAsJsonProcess(ConsoleProcess consoleProcess)
        {
            return new JsonProcessModel
            {
                RunAsConsole = true,

                VisibleOnStartup = consoleProcess.VisibleOnStartup,

                StartInDirectory = consoleProcess.StartupDirectory,
                ConsoleScript = consoleProcess.ConsoleScript,

                ProcessLabel = consoleProcess.ProcessLabel,
            };
        }

        /// <summary>
        /// Converts a <see cref="GUIProcess"/> to a <see cref="JsonProcessModel"/> 
        /// </summary>
        /// <param name="consoleProcess"></param>
        /// <returns></returns>
        private JsonProcessModel GUIProcessAsJsonProcess(GUIProcess guiProcess)
        {
            return new JsonProcessModel
            {
                RunAsConsole = false,

                VisibleOnStartup = guiProcess.VisibleOnStartup,

                ProcessPath = guiProcess.ProcessPath,
                ProcessArgs = guiProcess.ProcessArgs,

                ProcessLabel = guiProcess.ProcessLabel,
            };
        }


    };
};