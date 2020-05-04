namespace ProcessManager.UI
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;


    /// <summary>
    /// A class that inherits from <see cref="ICommand"/> and allows WPF command execution
    /// </summary>
    public class RelayCommand : ICommand
    {

        /// <summary>
        /// An event the fires when <see cref="CanExecute(object)"/> has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        #region Private fields

        /// <summary>
        /// Action that hold method signatures and invokes them if needed
        /// </summary>
        private Action _methodDelegate;

        /// <summary>
        /// A condition which will tell if the command should execute or not
        /// </summary>
        private Func<bool> _methodPredicate;

        /// <summary>
        /// A boolean flag that indicates if the command will be executed once until it finishes execution of <see cref="_methodDelegate"/>
        /// </summary>
        private readonly bool _singleFire;

        /// <summary>
        /// A boolean flag that indicates if this command is currently running
        /// </summary>
        private bool _isRunning;

        #endregion


        /// <summary>
        /// Default constrcutor
        /// </summary>
        /// <param name="action"> The action to execute </param>
        /// <param name="predicate"> A function predicate used to indicate if the control associated with this command will be enabled </param>
        /// <param name="singleFire"> A boolean flag that indicates if this function will run once until it completes before executiong again </param>
        public RelayCommand(Action action, Func<bool> predicate = null, bool singleFire = false)
        {
            // Checks if the method predicate is true or false and execute or disables the
            // button accordingly
            _methodPredicate = predicate;

            // Sets the delegate to the passed method
            _methodDelegate = action;

            _singleFire = singleFire;
        }


        public bool CanExecute(object parameter = null)
        {
            return _methodPredicate == null || _methodPredicate.Invoke();
        }


        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            // If this RelayCommand is using single fire mode
            if (_singleFire == true)
                // Check if this command is currently running
                if (_isRunning == true)
                    return;

            // Set running mode to true
            _isRunning = true;


            // Execute command
            try
            {
                _methodDelegate?.Invoke();
            }
            finally
            {
                // After command finishes reset flag
                _isRunning = false;
            };
        }

    }


    /// <summary>
    /// A generic version of the existing <see cref="RelayCommand"/> class 
    /// </summary>
    /// <typeparam name="T"> The type of aargument </typeparam>
    public class RelayCommand<T> : ICommand
    {
        /// <summary>
        /// An event the fires when <see cref="CanExecute(object)"/> has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        #region Private fields

        /// <summary>
        /// A condition which will tell if the command should execute or not
        /// </summary>
        private readonly Func<bool> _predicate;

        /// <summary>
        /// A "generic" method that takes a single argument
        /// </summary>
        private readonly Action<T> _method;

        /// <summary>
        /// A "generic" asynchronous method that takes a single argument
        /// </summary>
        private readonly Func<T, Task> _aasyncMethod;

        #endregion


        public RelayCommand(Action<T> method, Func<bool> predicate = null)
        {
            _method = method;
            _predicate = predicate;
        }

        public RelayCommand(Func<T, Task> method, Func<bool> predicate = null)
        {
            _aasyncMethod = method;
            _predicate = predicate;
        }

        public bool CanExecute(object parameter = null)
        {
            // Check if method predicate is null and invoke predicate if necessary
            return _predicate == null || _predicate.Invoke();
        }


        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="parameter"></param>
        public async void Execute(object parameter = null)
        {
            // Invoke if not null
            _method?.Invoke((T)parameter);

            try
            {
                if (!(_aasyncMethod is null))
                    await _aasyncMethod?.Invoke((T)parameter);
            }
            catch
            {
                throw;
            }
        }
    }

    /// <summary>
    /// An asynchronous "overload" of <see cref="RelayCommand"/>. Allows proper* execution of async Task commands
    /// </summary>
    public class AsyncRelayCommand : ICommand
    {

        #region Private fields

        /// <summary>
        /// An asynchronous function that will be executed
        /// </summary>
        private readonly Func<Task> _asyncAction;

        /// <summary>
        /// A condition which will tell if the command should execute or not
        /// </summary>
        private readonly Predicate<bool> _predicate;

        /// <summary>
        /// A boolean flag that indicates if the command will be executed once until it finishes execution of <see cref="_methodDelegate"/>
        /// </summary>
        private bool _singleFire;

        /// <summary>
        /// A boolean flag that indicates if this command is currently running
        /// </summary>
        private bool _isRunning;

        #endregion


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        /// <summary>
        /// Default constrcutor
        /// </summary>
        /// <param name="asyncAction"> The asynchrounous action to execute </param>
        /// <param name="predicate"> A function predicate used to indicate if the control associated with this command will be enabled </param>
        /// <param name="singleFire"> A boolean flag that indicates if this function will run once until it completes before executiong again </param>
        public AsyncRelayCommand(Func<Task> asyncAction, Predicate<bool> predicate = null, bool singleFire = false)
        {
            _asyncAction = asyncAction;
            _predicate = predicate;

            _singleFire = singleFire;
        }


        public bool CanExecute(object parameter)
        {
            return _predicate == null || _predicate.Invoke((bool)parameter);
        }


        public async void Execute(object parameter)
        {
            // If this RelayCommand is using single fire mode
            if (_singleFire == true)
                // Check if this command is currently running
                if (_isRunning == true)
                    return;

            // Set running mode to true
            _isRunning = true;


            // Execute command
            try
            {
                await _asyncAction?.Invoke();
            }
            finally
            {
                _isRunning = false;
            };
        }

    };

};