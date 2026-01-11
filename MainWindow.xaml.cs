using System.Windows;
using EVETranslate.ViewModels;

namespace EVETranslate
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}


