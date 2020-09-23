using System.ComponentModel;
using System.Windows;

namespace KanaLearn
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Model.PropertyChanged += Model_PropertyChanged;
        }

        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Running")
            {
                InputBox.Focus();
            }
        }
    }
}
