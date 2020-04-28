namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
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
        /// Method invoker that you can pass an object to
        /// </summary>
        private Action<object> _parameterMethodDelegate;

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that takes an <see cref="object"/> as a parameter and returns an await able <see cref="Task"/>
        /// </summary>
        private Func<object, Task> _parameterizedAsyncMethodDelegate;

        /// <summary>
        /// A <see cref="Func{T, TResult}"/> that returns an awaitable <see cref="Task"/>
        /// </summary>
        private Func<Task> _asyncMethodDelegate;

        /// <summary>
        /// An <see cref="IEnumerable{T}"/> that hold references to a actions
        /// </summary>
        private IEnumerable<Action> _actions;

        /// <summary>
        /// A condition which will tell if the command should execute or not
        /// </summary>
        private Func<bool> _methodPredicate;

		#endregion


		#region Constructors

		/// <summary>
		/// Main <see cref="RelayCommand"/> class constructor
		/// </summary>
		/// <param name="action"></param>
		/// <param name="predicate"></param>
		public RelayCommand(Action action, Func<bool> predicate = null)
		{
			// Checks if the method predicate is true or false and execute or disables the
			// button accordingly
			_methodPredicate = predicate;

			// Sets the delegate to the passed method
			_methodDelegate = action;
		}

		/// <summary>
		/// <see cref="RelayCommand"/> constructor that will accept passed parameters
		/// </summary>
		/// <param name="action"></param>
		/// <param name="predicate"></param>
		public RelayCommand(Action<object> parameterAction, Func<bool> predicate = null)
        {
            // Checks if the method predicate is true or false and execute or disables the
            // button accordingly
            _methodPredicate = predicate;

            // Sets the delegate to the passed method
            _parameterMethodDelegate = parameterAction;
        }


        /// <summary>
        /// Allows execution of awaitable Async methods with a <see cref="Task"/> return type
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <param name="predicate"></param>
        public RelayCommand(Func<Task> asyncAction, Func<bool> predicate = null)
        {
            // Checks if the method predicate is true or false and execute or disables the
            // button accordingly
            _methodPredicate = predicate;

            // Sets the delegate to the passed method
            _asyncMethodDelegate = asyncAction;
        }

        /// <summary>
        /// Executes an awaitable Async method with a <see cref="Task"/> return type and allows passing of parameters
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <param name="predicate"></param>
        public RelayCommand(Func<object, Task> asyncAction, Func<bool> predicate = null)
        {
            // Checks if the method predicate is true or false and execute or disables the
            // button accordingly
            _methodPredicate = predicate;

            // Sets the delegate to the passed method
            _parameterizedAsyncMethodDelegate = asyncAction;
        }


        /// <summary>
        /// Executes multiple actions at once (depending on order of executions)
        /// </summary>
        /// <param name="actions"></param>
        /// <param name="predicate"></param>
        public RelayCommand(IEnumerable<Action> actions, Func<bool> predicate = null)
        {
            // Checks if the method predicate is true or false and execute or disables the
            // button accordingly
            _methodPredicate = predicate;

            // Sets the delegate to the passed method
            _actions = actions;
        }

        #endregion


        public bool CanExecute(object parameter = null)
        {
            return _methodPredicate == null || _methodPredicate.Invoke();
        }


        /// <summary>
        /// Executes the action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter = null)
        {
            // If the passed parameter is null
            if (parameter is null)
            {
                // Invokes the parameter-less delegate
                _methodDelegate?.Invoke();
                _asyncMethodDelegate?.Invoke();


                // If the actions enumerable has commands
                if (!(_actions is null))
                {
                    // Go Through every actions that is held in the Actions enumerable
                    foreach (Action action in _actions)
                    {
                        // If the action isn't null invoke
                        action?.Invoke();
                    };
                };
            }
            else
            {
                // Invokes the delegate that contains the parameters
                _parameterMethodDelegate?.Invoke(parameter);

                _parameterizedAsyncMethodDelegate?.Invoke(parameter);
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
        public void Execute(object parameter = null)
        {
            // Invoke if not null
            _method?.Invoke((T)parameter);
            _aasyncMethod?.Invoke((T)parameter);
        }
    }
}
