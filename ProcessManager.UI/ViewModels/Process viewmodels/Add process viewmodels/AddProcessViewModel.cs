namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;



    /// <summary>
    /// 
    /// </summary>
    public class AddProcessViewModel
    {
        public static AddProcessViewModel DesignInstance => new AddProcessViewModel(
            new ProjectItemViewModel(new Project()
            {
                ProjectPath = "C:\\Software\\Super secret project",
            }));


        public ProjectItemViewModel Project { get; }

        public List<ProcessType> ProcessTypes { get; private set; }


        public ICommand SwitchToProjectPageCommand { get; }
        public ICommand SwitchToMainPageCommand { get; }
        public ICommand OpenProjectDirectoryCommand { get; }

        public ICommand SwitchToAddProcessViewCommand { get; }

        public AddProcessViewModel(ProjectItemViewModel project)
        {
            Project = project;

            SetupProcessTypes();

            SwitchToProjectPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(Project));

            SwitchToMainPageCommand = new RelayCommand(() =>
            DI.MainWindowViewModel.CurrentView = new ProjectListView(new ProjectsListViewModel(DI.Projects)));

            OpenProjectDirectoryCommand = new RelayCommand(() =>
            DI.FolderDialog.OpenFolder(Project.Project.ProjectPath));

            SwitchToAddProcessViewCommand = new RelayCommand<ProcessType>(ExecuteSwitchToAddProcessViewCommand);
        }


        private void ExecuteSwitchToAddProcessViewCommand(ProcessType processType)
        {
            // Switch to the appropriate "Add(processType)View 
            switch (processType)
            {
                case ProcessType.Console:
                {
                    DI.MainWindowViewModel.CurrentView = new AddConsoleProcessView(new AddConsoleProcessViewModel(Project));
                    break;
                };

                case ProcessType.GUI:
                {
                    DI.MainWindowViewModel.CurrentView = new AddGUIProcessView(new AddGUIProcessViewModel(Project));
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