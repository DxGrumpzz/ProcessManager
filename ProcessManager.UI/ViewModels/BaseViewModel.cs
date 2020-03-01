namespace ProcessManager.UI
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// 
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
