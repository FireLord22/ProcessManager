using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProcessM.Core.Models;
using ProcessM.Core.Services;

namespace ProcessM.Core.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;

        public ObservableCollection<ProcessTreeNode> Processes { get; }
            = new ObservableCollection<ProcessTreeNode>();

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

        public MainViewModel() : this(new ProcessService())
        {
        }

        public MainViewModel(IProcessService service)
        {
            _service = service;
        }

        public async Task RefreshAsync()
        {
            // Получаем "полные" данные
            var processInfoList = await Task.Run(() => _service.GetProcesses());

            // Преобразуем в UI-модель
            var uiList = processInfoList.Select(p => new ProcessTreeNode
            {
                Id = p.Id,
                Name = p.Name,
                Memory = p.MemoryUsage,
                Priority = p.Priority
            }).ToList();

            // Передаем в метод синхронизации коллекции
            SyncCollection(uiList);
        }

        public void SyncCollection(System.Collections.Generic.List<ProcessTreeNode> newList)
        {
            foreach (var proc in newList)
            {
                var existing = Processes.FirstOrDefault(p => p.Id == proc.Id);

                if (existing == null)
                    Processes.Add(proc);
                else
                {
                    existing.Memory = proc.Memory;
                    existing.Name = proc.Name;
                    existing.Priority = proc.Priority;
                }
            }

            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!newList.Any(p => p.Id == Processes[i].Id))
                    Processes.RemoveAt(i);
            }
        }

        public void KillSelected()
        {
            if (SelectedProcess != null)
                _service.KillProcess(SelectedProcess.Id);
        }

        public void ChangePriority(ProcessPriorityClass priority)
        {
            if (SelectedProcess != null)
                _service.SetPriority(SelectedProcess.Id, priority);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}