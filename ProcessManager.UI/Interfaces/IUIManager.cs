namespace ProcessManager.UI
{
    using System;
    using System.Collections.Generic;
    using System.Text;


    /// <summary>
    /// An interface that describes an implementation of a WPF ui manager
    /// </summary>
    public interface IUIManager
    {
        /// <summary>
        /// Changes the app's current view 
        /// </summary>
        /// <typeparam name="T"> The type of viewmodel </typeparam>
        /// <param name="view"> The view to change to </param>
        /// <param name="viewmodel"> The view's associated viewmodel </param>
        public void ChangeView<TViewModel>(View view, TViewModel viewmodel) 
            where TViewModel : BaseViewModel;

    };
};