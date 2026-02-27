using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ProcessM.Core.Services;
using ProcessM.Core.ViewModels;

namespace ProcessManager.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel(new ProcessService());
            DataContext = _vm;

            StartLoop();
        }

        private async void StartLoop()
        {
            while (true)
            {
                await _vm.RefreshAsync();
                await Task.Delay(2000);
            }
        }

        private void Kill_Click(object sender, RoutedEventArgs e)
        {
            _vm.KillSelected();
        }

        private void Priority_Click(object sender, RoutedEventArgs e)
        {
            if (priorityBox.SelectedItem is ComboBoxItem item)
            {
                var priority = (ProcessPriorityClass)
                    System.Enum.Parse(typeof(ProcessPriorityClass), item.Content.ToString());

                _vm.ChangePriority(priority);
            }
        }

        private void ApplyAffinity_Click(object sender, RoutedEventArgs e)
        {
            _vm.ApplyAffinity();
        }
    }
}