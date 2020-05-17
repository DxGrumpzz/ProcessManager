namespace ProcessManager.UI
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AddProcessView.xaml
    /// </summary>
    public partial class AddProcessView : UserControl
    {

        public AddProcessView()
        {
            InitializeComponent();
        }

        public AddProcessView(AddProcessViewModel viewModel)
        {
            DataContext = viewModel;
            InitializeComponent();
        }

    };
};