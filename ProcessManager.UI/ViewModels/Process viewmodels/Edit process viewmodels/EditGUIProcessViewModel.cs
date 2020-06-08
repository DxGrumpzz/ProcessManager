namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class EditGUIProcessViewModel : EditProcessBaseViewModel
    {

        public static EditGUIProcessViewModel DesignInstance => new EditGUIProcessViewModel(
            new ProjectItemViewModel(new Project()
            {
                ProjectPath = @"C:\Development\npm test",
            }),
            new ProcessItemViewModel(new ProjectItemViewModel(new Project()
            {
                ProjectPath = @"C:\Development\npm test",
            }),
            new GUIProcess(@"C:\Software\VScode.exe")))
        {
            
        };



        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _selectedPath;

        #endregion


        #region Public properties

        /// <summary>
        /// A path that the user has selected for this process
        /// </summary>
        public string SelectedPath
        {
            get => _selectedPath;
            set
            {
                _selectedPath = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// A new string of arguments for this process
        /// </summary>
        public string Arguments { get; set; }

        /// <summary>
        /// A new text label for this process
        /// </summary>
        public string ProcessLabel { get; set; }

        /// <summary>
        /// A bool flag that indicates if this process will be visible on startup
        /// </summary>
        public bool ProcessVisibleOnStartup { get; set; }

        #endregion


        #region Commands

        public ICommand SelectProcessPathCommand { get; }

        #endregion


        public EditGUIProcessViewModel(ProjectItemViewModel projectViewModel, ProcessItemViewModel processItemViewModel)
        {
            var process = ValidateProcessType<GUIProcess>(processItemViewModel.Process);

            ProcessVM = processItemViewModel;
            ProjectItemVM = projectViewModel;

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
            var project = ProjectItemVM.Project;

            // Find process
            int index = project.ProcessList.IndexOf(ProcessVM.Process);

            // Check if no process found
            if (index == -1)
                return;

            // Update process reference directly with the new edits
            project.ProcessList[index] = new GUIProcess(SelectedPath, Arguments, ProcessVisibleOnStartup)
            {
                ProcessLabel = ProcessLabel,
            };

            // Serialize project list 
            var projectBytes = DI.Serializer.SerializeProcessList(project.ProcessList);

            // Update project config file
            File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);

            // Switch back to the 
            DI.UI.ChangeView(View.ProjectItemView, ProjectItemVM);
        }
    };
};