namespace ProcessManager.UI
{
    using System.Windows.Controls;

    /// <summary>
    /// Interaction logic for EditProcessView.xaml
    /// </summary>
    public partial class EditGUIProcessView : UserControl
    {

        public EditGUIProcessView()
        {
            InitializeComponent();
        }

        public EditGUIProcessView(EditGUIProcessViewModel editProcessVM)
        {
            InitializeComponent();
            DataContext = editProcessVM;
        }

    };
};