namespace ProcessManager.UI
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Windows.Controls;

    /// <summary>
    /// The ViewModel for MainWindow
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public static MainWindowViewModel DesignInstance => new MainWindowViewModel(
            new Project[] 
            {
                new Project()
                {
                    ProjectPath = $@"C:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
                },

                new Project()
               {
                   ProjectPath = $@"D:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
               },

                new Project()
               {
                   ProjectPath = $@"A:\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}\{Path.GetFileNameWithoutExtension(Path.GetRandomFileName())}",
               },
            })
        {

            Projects = new ProjectItemViewModel[] { },
            CurrentView = new MainView(new MainWindowViewModel(new Project[] { })),
        };


        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContentControl _currentView = new MainView();

        #endregion


        #region Public properties

        /// <summary>
        /// A list of projects
        /// </summary>
        public IEnumerable<ProjectItemViewModel> Projects { get; set; }

        /// <summary>
        /// The page currently being displayed
        /// </summary>
        public ContentControl CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        #endregion


        public MainWindowViewModel(IEnumerable<Project> projects)
        {
            Projects = projects.Select(project =>
            {
                return new ProjectItemViewModel(project);
            });
        }

    };
};
