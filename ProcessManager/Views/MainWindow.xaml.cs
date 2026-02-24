using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using ProcessManager.ViewModels;

namespace ProcessManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)DataContext).KillSelected();
        }

        private void Priority_Click(object sender, RoutedEventArgs e)
        {
            if (priorityBox.SelectedItem is ComboBoxItem item)
            {
                var priority = (ProcessPriorityClass)
                    System.Enum.Parse(typeof(ProcessPriorityClass), item.Content.ToString());

                ((MainViewModel)DataContext).ChangePriority(priority);
            }
        }
    }
}