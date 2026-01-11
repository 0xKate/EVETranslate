using EVETranslate.Models;
using EVETranslate.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EVETranslate
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TabItem tabItem)
                return;

            // DataContext of the TabItem is the item from Tabs
            if (tabItem.DataContext is AddTabPlaceholder)
            {
                // Prevent the tab from being selected
                e.Handled = true;

                if (DataContext is MainViewModel vm)
                {
                    vm.AddTabCommand.Execute(null);
                }
            }
        }

    }
}


