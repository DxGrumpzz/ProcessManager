﻿namespace ProcessManager.UI
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
        public ProcessItemViewModel ProcessVM { get; private set; }

        /// <summary>
        /// A project viewmodel associated with the process
        /// </summary>
        public ProjectItemViewModel ProjectItemVM { get; private set; }

        public ProjectHeaderViewModel ProjectHeaderVM { get; private set; }

        #endregion


        #region Public commands

        public ICommand SaveProcessCommand { get; }
        public ICommand DeleteProcessCommand { get; }

        public ICommand BackToProjectPageCommand { get; }
        public ICommand BackToMainPageCommand { get; }

        public ICommand OpenProjectDirectoryCommand { get; }

        #endregion


        protected EditProcessBaseViewModel(ProjectItemViewModel projectViewModel, ProcessItemViewModel processItemViewModel)
        {
            ProjectItemVM = projectViewModel;

            ProcessVM = processItemViewModel;

            ProjectHeaderVM = new ProjectHeaderViewModel(projectViewModel.Project);


            SaveProcessCommand = new RelayCommand(ExecuteSaveProcessCommand);
            DeleteProcessCommand = new RelayCommand(ExecuteDeleteProcessCommand);


            BackToProjectPageCommand = new RelayCommand(() =>
            DI.UI.ChangeView(View.ProjectItemView, ProjectItemVM));

            BackToMainPageCommand = new RelayCommand(() =>
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM));


            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(ProjectItemVM.Project.ProjectPath));

        }


        protected virtual void ExecuteDeleteProcessCommand()
        {
            // Ask for usre confirmation
            var result = DI.UserDialog.ShowChoiceDialog($"Are you absolutley sure you want to delete this process ?", "Delete process confirmation");
            
            // If user decides to delete this process
            if(result == UserDialogResult.Yes)
            {
                var process = ProcessVM.Process;
                var project = ProjectItemVM.Project;

                // Remove this process from the main list
                project.ProcessList.Remove(process);

                // Update projects list view
                DI.ProjectsListVM.UpdateProjectsList();

                // Save changes in project config
                DI.FileManager.UpdateProjectConfig(project);

                // Switch back to the project's view
                DI.UI.ChangeView(View.ProjectItemView, ProjectItemVM);
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