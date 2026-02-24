using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using ProcessManager.Models;
using ProcessManager.Services;

namespace ProcessManager.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ProcessTreeNode> Processes { get; } =
            new ObservableCollection<ProcessTreeNode>();

        private ProcessTreeNode _selectedProcess;
        public ProcessTreeNode SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcess));
            }
        }

        public MainViewModel()
        {
            StartLoop();
        }

        private async void StartLoop()
        {
            while (true)
            {
                var list = await Task.Run(() => ProcessService.GetProcesses());

                Application.Current.Dispatcher.Invoke(() =>
                {
                    SyncCollection(list);
                });

                await Task.Delay(2000);
            }
        }

        /// <summary>
        /// СИНХРОНИЗАЦИЯ БЕЗ СБРОСА ВЫБОРА
        /// </summary>
        private void SyncCollection(System.Collections.Generic.List<ProcessTreeNode> newList)
        {
            // Обновляем существующие
            foreach (var proc in newList)
            {
                var existing = Processes.FirstOrDefault(p => p.Id == proc.Id);

                if (existing == null)
                {
                    Processes.Add(proc);
                }
                else
                {
                    existing.Memory = proc.Memory;
                    existing.Name = proc.Name;
                }
            }

            // Удаляем закрытые процессы
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!newList.Any(p => p.Id == Processes[i].Id))
                    Processes.RemoveAt(i);
            }
        }

        public void KillSelected()
        {
            if (SelectedProcess != null)
                ProcessService.KillProcess(SelectedProcess.Id);
        }

        public void ChangePriority(ProcessPriorityClass priority)
        {
            if (SelectedProcess != null)
                ProcessService.SetPriority(SelectedProcess.Id, priority);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string n) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}