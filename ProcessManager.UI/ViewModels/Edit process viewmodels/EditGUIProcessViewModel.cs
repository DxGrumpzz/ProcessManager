namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class EditGUIProcessViewModel : EditProcessBaseViewModel
    {

        public static EditGUIProcessViewModel DesignInstance => new EditGUIProcessViewModel()
        {

        };



        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _selectedPath;

        #endregion


        #region Public properties

        public string SelectedPath 
        { 
            get => _selectedPath; 
            set
            {
                _selectedPath = value;
                OnPropertyChanged();
            }
        }

        public string Arguments { get; set; }

        public string ProcessLabel { get; set; }

        public bool ProcessVisibleOnStartup { get; set; }

        #endregion


        #region Commands

        public ICommand SelectProcessPathCommand { get; }

        #endregion


        private EditGUIProcessViewModel() { }
        public EditGUIProcessViewModel(ProjectItemViewModel projectViewModel, ProcessItemViewModel processItemViewModel)
        {
            if (!(processItemViewModel.Process is GUIProcess process))
            {
                Debugger.Break();
                throw new Exception($"Invalid process supplied.\n" +
                    $"{processItemViewModel.Process} is {processItemViewModel.Process.GetType()}, expected {nameof(GUIProcess)}");
            };

            ProcessVM = processItemViewModel;
            ProjectVM = projectViewModel;

            SelectedPath = process.ProcessPath;
            Arguments = process.ProcessArgs;
            ProcessLabel = process.ProcessLabel;
            ProcessVisibleOnStartup = process.VisibleOnStartup;

            SelectProcessPathCommand = new RelayCommand(ExecuteSelectProcessPathCommand);
        }


        private void ExecuteSelectProcessPathCommand()
        {
            var fileDialog = DI.FileDialog;

            if (fileDialog.ShowOpenFileDialog() == true)
                SelectedPath = fileDialog.SelectedFilePath;
        }

        protected override void ExecuteSaveProcessCommand()
        {
            var process = (GUIProcess)ProcessVM.Process;
            var project = ProjectVM.Project;

            int index = project.ProcessList.IndexOf(ProcessVM.Process);

            if(index != -1)
            {
                project.ProcessList[index] = new GUIProcess(SelectedPath, Arguments, ProcessVisibleOnStartup)
                {
                    ProcessLabel = ProcessLabel,
                };
            };

            var projectBytes = DI.Serializer.SerializeToString(
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

            File.WriteAllText(project.ProjectPathWithConfig, projectBytes);

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