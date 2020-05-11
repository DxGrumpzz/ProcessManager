namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for EditProcessView.xaml
    /// </summary>
    public partial class EditConsoleProcessView : UserControl
    {

        public EditConsoleProcessView()
        {
            InitializeComponent();
        }

        public EditConsoleProcessView(EditConsoleProcessViewModel editProcessVM)
        {
            InitializeComponent();
            DataContext = editProcessVM;
        }

    };
};