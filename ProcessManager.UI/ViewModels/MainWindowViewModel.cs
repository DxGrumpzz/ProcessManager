namespace ProcessManager.UI
{
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Controls;

    /// <summary>
    /// The ViewModel for MainWindow
    /// </summary>
    public class MainWindowViewModel : BaseViewModel
    {
        public static MainWindowViewModel DesignInstance => new MainWindowViewModel()
        {
            CurrentView = new ProjectsView(ProjectsListViewModel.DesignInstance),
        };

        #region Private fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ContentControl _currentView = new ProjectsView();

        #endregion


        #region Public properties

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

    };
};