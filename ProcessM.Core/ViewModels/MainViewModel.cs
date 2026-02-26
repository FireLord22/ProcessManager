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

        public ObservableCollection<ProcessInfo> Processes { get; }
            = new ObservableCollection<ProcessInfo>();

        private ProcessInfo _selectedProcess;
        public ProcessInfo SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcess));
            }
        }

        // Для XAML
        public MainViewModel() : this(new ProcessService())
        {
        }

        // Для тестов / DI
        public MainViewModel(IProcessService service)
        {
            _service = service;
        }

        /// <summary>
        /// Обновляет список процессов (вызывается из UI)
        /// </summary>
        public async Task RefreshAsync()
        {
            var list = await Task.Run(() => _service.GetProcesses());
            SyncCollection(list);
        }

        /// <summary>
        /// Синхронизация ObservableCollection с новым списком
        /// </summary>
        public void SyncCollection(System.Collections.Generic.List<ProcessInfo> newList)
        {
            // Обновление и добавление
            foreach (var proc in newList)
            {
                var existing = Processes.FirstOrDefault(p => p.Id == proc.Id);

                if (existing == null)
                {
                    Processes.Add(proc);
                }
                else
                {
                    existing.Name = proc.Name;
                    existing.MemoryUsage = proc.MemoryUsage;
                    existing.ThreadCount = proc.ThreadCount;
                    existing.Priority = proc.Priority;
                    existing.CpuTime = proc.CpuTime;
                    existing.AffinityMask = proc.AffinityMask;
                    existing.Threads = proc.Threads;
                }
            }

            // Удаление завершённых процессов
            for (int i = Processes.Count - 1; i >= 0; i--)
            {
                if (!newList.Any(p => p.Id == Processes[i].Id))
                    Processes.RemoveAt(i);
            }
        }

        /// <summary>
        /// Завершить выбранный процесс
        /// </summary>
        public void KillSelected()
        {
            if (SelectedProcess != null)
                _service.KillProcess(SelectedProcess.Id);
        }

        /// <summary>
        /// Изменить приоритет выбранного процесса
        /// </summary>
        public void ChangePriority(ProcessPriorityClass priority)
        {
            if (SelectedProcess != null)
                _service.SetPriority(SelectedProcess.Id, priority);
        }

        /// <summary>
        /// Проверка: включено ли ядро CPU для выбранного процесса
        /// </summary>
        public bool IsCoreEnabled(int coreIndex)
        {
            if (SelectedProcess == null)
                return false;

            return AffinityHelper.IsCoreEnabled(SelectedProcess.AffinityMask, coreIndex);
        }

        /// <summary>
        /// Установить affinity выбранного процесса
        /// </summary>
        public void SetAffinity(bool[] cores)
        {
            if (SelectedProcess == null)
                return;

            var mask = AffinityHelper.BuildMask(cores);

            try
            {
                var process = Process.GetProcessById(SelectedProcess.Id);
                process.ProcessorAffinity = mask;
                SelectedProcess.AffinityMask = mask;
                OnPropertyChanged(nameof(SelectedProcess));
            }
            catch
            {
                // процесс мог завершиться или нет прав
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}