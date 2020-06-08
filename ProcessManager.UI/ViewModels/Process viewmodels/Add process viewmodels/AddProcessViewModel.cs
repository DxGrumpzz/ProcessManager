namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;



    /// <summary>
    /// 
    /// </summary>
    public class AddProcessViewModel : BaseViewModel
    {
        public static AddProcessViewModel DesignInstance => new AddProcessViewModel(
            new ProjectItemViewModel(new Project()
            {
                ProjectPath = "C:\\Software\\Super secret project",
            }));


        public ProjectItemViewModel ProjectItemVM { get; }

        public List<ProcessType> ProcessTypes { get; private set; }


        public ICommand SwitchToProjectPageCommand { get; }
        public ICommand SwitchToMainPageCommand { get; }
        public ICommand OpenProjectDirectoryCommand { get; }

        public ICommand SwitchToAddProcessViewCommand { get; }

        public ICommand OpenProjectDirectory { get; }

        public AddProcessViewModel(ProjectItemViewModel project)
        {
            ProjectItemVM = project;

            SetupProcessTypes();

            SwitchToProjectPageCommand = new RelayCommand(() =>
            DI.UI.ChangeView(View.ProjectItemView, ProjectItemVM));

            SwitchToMainPageCommand = new RelayCommand(() =>
            DI.UI.ChangeView(View.ProjectsListView, DI.ProjectsListVM));

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(ProjectItemVM.Project.ProjectPath));

            SwitchToAddProcessViewCommand = new RelayCommand<ProcessType>(ExecuteSwitchToAddProcessViewCommand);

            OpenProjectDirectory = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(ProjectItemVM.Project.ProjectPath));

        }


        private void ExecuteSwitchToAddProcessViewCommand(ProcessType processType)
        {
            // Switch to the appropriate "Add(processType)View 
            switch (processType)
            {
                case ProcessType.Console:
                {
                    DI.UI.ChangeView(View.AddConsoleProcessView, new AddConsoleProcessViewModel(ProjectItemVM));
                    break;
                };

                case ProcessType.GUI:
                {
                    DI.UI.ChangeView(View.AddGUIProcessView, new AddGUIProcessViewModel(ProjectItemVM));
                    break;
                };
            };

        }

        private void SetupProcessTypes()
        {
            var processTypes = Enum.GetValues(typeof(ProcessType));
            ProcessTypes = new List<ProcessType>(processTypes.Length);

            foreach (ProcessType processType in processTypes)
            {
                ProcessTypes.Add(processType);
            };

        }

    };
};