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
    public abstract class EditProcessBaseViewModel : BaseViewModel
    {
        
        public ProcessItemViewModel ProcessVM { get; set; }

        public ProjectItemViewModel ProjectVM { get; set; }


        public ICommand SaveProcessCommand { get; }
        public ICommand DeleteProcessCommand { get; }

        public ICommand BackToProjectPageCommand { get; }
        public ICommand BackToMainPageCommand { get; }



        protected EditProcessBaseViewModel()
        {
            SaveProcessCommand = new RelayCommand(ExecuteSaveProcessCommand);
            DeleteProcessCommand = new RelayCommand(ExecuteDeleteProcessCommand);


            BackToProjectPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM));

            BackToProjectPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects)));
        }

        protected virtual void ExecuteDeleteProcessCommand()
        {
            var result = DI.UserDialog.ShowChoiceDialog($"Are you absolutley sure you want to delete this process ?", "Delete process confirmation");
            
            if(result == UserDialogResult.Yes)
            {
                var process = (GUIProcess)ProcessVM.Process;
                var project = ProjectVM.Project;

                project.ProcessList.Remove(process);

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
            };
        }

        protected abstract void ExecuteSaveProcessCommand();


        /// <summary>
        /// Validates that a given <see cref="IProcessModel"/> is of correct type
        /// </summary>
        /// <typeparam name="TProcess"> The type of expected process </typeparam>
        /// <param name="process"> The actual process </param>
        /// <returns></returns>
        protected TProcess ValidateProcessType<TProcess>(IProcessModel process)
            where TProcess : IProcessModel
        {
            // Using pattern matching, check if process 'is a' TProcess
            if (!(process is TProcess))
            {
                Debugger.Break();
                throw new Exception($"Invalid process supplied.\n" +
                    $"{nameof(process)} is {process.GetType()}, expected {typeof(TProcess)}");
            };

            return (TProcess)process;
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