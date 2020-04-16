namespace ProcessManager.UI
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A base class that all ViewModels which have a Runtime-changing value should inherit from
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName()]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    };
};
