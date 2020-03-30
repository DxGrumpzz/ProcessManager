namespace ProcessManager.UI
{
    using System;
    using System.IO;
    using System.Windows.Input;


    /// <summary>
    /// 
    /// </summary>
    public class ProjectItemViewModel
    {
        public static ProjectItemViewModel DesignInstance => new ProjectItemViewModel(new Project()
        {
            ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
        });


        public Project Project { get; }

        public ICommand GotoProjectViewCommnad { get; }
        public ICommand GotoMainPageCommnad { get; }


        public ProjectItemViewModel(Project project)
        {
            Project = project;

            GotoProjectViewCommnad = new RelayCommand(ExecuteGotoProjectViewCommnad);
            GotoMainPageCommnad = new RelayCommand(ExecuteGotoMainPageommnad);
        }

        private void ExecuteGotoMainPageommnad()
        {
            DI.MainWindowViewModel.CurrentView = new MainView(DI.MainWindowViewModel);
        }

        private void ExecuteGotoProjectViewCommnad()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(this);
        }
    };
};
