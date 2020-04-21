namespace ProcessManager.UI
{
    using System;
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for AddConsoleProcessView.xaml
    /// </summary>
    public partial class AddConsoleProcessView : UserControl
    {

        public AddConsoleProcessView()
        {
            InitializeComponent();
        }

        public AddConsoleProcessView(AddConsoleProcessViewModel projectVM) : 
            this()
        {
            DataContext = projectVM;
        }

    };
};