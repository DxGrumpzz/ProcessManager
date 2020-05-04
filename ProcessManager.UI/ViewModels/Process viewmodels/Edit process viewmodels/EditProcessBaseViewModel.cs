namespace ProcessManager.UI
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public abstract class EditProcessBaseViewModel : BaseViewModel
    {

        #region Public properties

        /// <summary>
        /// A process VM associated with this viewmodel
        /// </summary>
        public ProcessItemViewModel ProcessVM { get; set; }

        /// <summary>
        /// A project viewmodel associated with the process
        /// </summary>
        public ProjectItemViewModel ProjectVM { get; set; }


        #endregion


        #region Public commands

        public ICommand SaveProcessCommand { get; }
        public ICommand DeleteProcessCommand { get; }

        public ICommand BackToProjectPageCommand { get; }
        public ICommand BackToMainPageCommand { get; }


        #endregion


        protected EditProcessBaseViewModel()
        {
            SaveProcessCommand = new RelayCommand(ExecuteSaveProcessCommand);
            DeleteProcessCommand = new RelayCommand(ExecuteDeleteProcessCommand);


            BackToProjectPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(ProjectVM));

            BackToMainPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects)));
        }


        protected virtual void ExecuteDeleteProcessCommand()
        {
            // Ask for usre confirmation
            var result = DI.UserDialog.ShowChoiceDialog($"Are you absolutley sure you want to delete this process ?", "Delete process confirmation");
            
            // If user decides to delete this process
            if(result == UserDialogResult.Yes)
            {
                var process = (GUIProcess)ProcessVM.Process;
                var project = ProjectVM.Project;

                // Remove this process from the main list
                project.ProcessList.Remove(process);

                // Serialize the list after removing the process
                var projectBytes = DI.Serializer.SerializeProcessList(project.ProcessList);
               
                // Update the project's config file
                File.WriteAllBytes(project.ProjectPathWithConfig, projectBytes);

                // Switch back to the project's view
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

    };
};