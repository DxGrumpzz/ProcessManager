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
    public class AddProcessViewModel : BaseViewModel
    {
        public static AddProcessViewModel DesignInstance => new AddProcessViewModel()
        {
            SelectedProcessPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetRandomFileName()}",
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _selectedProcessPath;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool _addProcessEnabled;

        #endregion


        #region Public properties

        /// <summary>
        /// A ProjectViewModel which is used when the user wants to go back to the Project view and not lose any saved data
        /// </summary>
        public ProjectItemViewModel ProjectVM { get; }


        public string SelectedProcessPath
        {
            get => _selectedProcessPath;
            set
            {
                _selectedProcessPath = value;
                OnPropertyChanged();
            }
        }


        public string ProcessAgs { get; set; } = string.Empty;

        public string[] ProcessAgsSplit => ProcessAgs?.Split(' ');


        public bool ProcessVisibleOnStartup { get; set; }

        public string ProcessLabel { get; set; } = string.Empty;

        public bool AddProcessEnabled
        {
            get => _addProcessEnabled;
            set
            {
                _addProcessEnabled = value;
                OnPropertyChanged();
            }
        }


        #endregion


        #region Commands

        public ICommand SelectProcessCommand { get; }
        public ICommand BackToMainPageCommand { get; }
        public ICommand BackToProjectPageCommand { get; }
        public ICommand AddProcessCommand { get; }

        #endregion


        private AddProcessViewModel() { }
        public AddProcessViewModel(ProjectItemViewModel projectVM)
        {
            ProjectVM = projectVM;

            SelectProcessCommand = new RelayCommand(ExecuteSelectProcessCommand);

            BackToMainPageCommand = new RelayCommand(ExecuteBackToMainPageCommand);
            BackToProjectPageCommand = new RelayCommand(ExecuteBackToProjectPageCommand);

            AddProcessCommand = new RelayCommand(ExecuteAddProcessCommand);
        }

        private void ExecuteAddProcessCommand()
        {
            var project = ProjectVM.Project;

            project.ProcessList.Add(new ProcessModel()
            {
                ProcessPath = SelectedProcessPath,
                VisibleOnStartup = ProcessVisibleOnStartup,
                ProcessArgs = ProcessAgs,
                ProcessLabel = ProcessLabel,
            });

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

            // Return back to the Project's view
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }

        private void ExecuteSelectProcessCommand()
        {
            // Use the open file dialog to select a process
            var fileDialog = DI.FileDialog;

            // If user has selected a process
            if (fileDialog.ShowOpenFileDialog() == true)
            {
                // Set selected path
                SelectedProcessPath = fileDialog.GetOpenFileDialogRresult();

                // Vertify that the selected path is valid
                if (string.IsNullOrWhiteSpace(SelectedProcessPath) == false &&
                    File.Exists(SelectedProcessPath) == true)
                {
                    // Allow user to add the process
                    AddProcessEnabled = true;
                }
                else
                    AddProcessEnabled = false;
            };

        }

        private void ExecuteBackToMainPageCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects));
        }

        private void ExecuteBackToProjectPageCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM);
        }

    };
};