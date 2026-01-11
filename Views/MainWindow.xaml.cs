using EVETranslate.Models;
using EVETranslate.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace EVETranslate.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TabItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is not TabItem tabItem)
                return;

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


