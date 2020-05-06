namespace ProcessManager.UI
{
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;

    /// <summary>
    /// 
    /// </summary>
    public class ProjectListItemViewModel : BaseViewModel
    {
        public static ProjectListItemViewModel DesignInstance => new ProjectListItemViewModel(
            new Project()
            {
                ProjectPath = @"C:\Software\Secret Project",
            })
        {
            SettingsButtonVisible = true,
        };


        public Project Project { get; }

        private bool _settingsButtonVisible;


        public bool SettingsButtonVisible
        {
            get => _settingsButtonVisible;
            set
            {
                _settingsButtonVisible = value;
                OnPropertyChanged();
            }
        }



        #region Public commands


        public ICommand RunProjectCommand { get; }
        public ICommand CloseProjectCommand { get; }

        public ICommand MouseEnterCommand { get; }
        public ICommand MouseLeaveCommand { get; }

        public ICommand SwitchToProjectViewCommand { get; }

        #endregion



        private ProjectListItemViewModel() { }
        public ProjectListItemViewModel(Project project)
        {
            Project = project;

            RunProjectCommand = new RelayCommand(ExecuteRunProjectCommand, singleFire: true);
            CloseProjectCommand = new RelayCommand(ExecuteCloseProjectCommand, singleFire: true);

            SwitchToProjectViewCommand = new RelayCommand(ExecuteSwitchToProjectViewCommand);

            MouseEnterCommand = new RelayCommand(() =>
            {
                SettingsButtonVisible = true;
            });

            MouseLeaveCommand = new RelayCommand(() =>
            {
                SettingsButtonVisible = false;
            });
        }


        private void ExecuteRunProjectCommand()
        {
            foreach (var process in Project.ProcessList)
            {
                Task.Run(process.RunProcess);
            };
        }

        private void ExecuteCloseProjectCommand()
        {
            // Only close running processes 
            var runningProcesses = Project.ProcessList.Where(process => process.IsRunning == true);

            foreach (var process in runningProcesses)
            {
                Task.Run(process.CloseProcess);
            };
        }

        private void ExecuteSwitchToProjectViewCommand()
        {
            DI.MainWindowViewModel.CurrentView = new ProjectItemView(new ProjectItemViewModel(Project));
        }

    };
};