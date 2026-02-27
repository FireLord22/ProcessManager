using ProcessM.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ProcessM.Core.Services;

namespace ProcessM.Core.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IProcessService _service;

        public ObservableCollection<ProcessTreeNode> Processes { get; }
            = new ObservableCollection<ProcessTreeNode>();

        public ObservableCollection<ThreadInfo> Threads { get; } = new ObservableCollection<ThreadInfo>();

        public ObservableCollection<CpuCore> Cores { get; }
            = new ObservableCollection<CpuCore>();

        private ProcessTreeNode _selectedProcess;
        public ProcessTreeNode SelectedProcess
        {
            get => _selectedProcess;
            set
            {
                _selectedProcess = value;
                OnPropertyChanged(nameof(SelectedProcess));
                LoadAffinity();
                LoadThreads();
            }
        }

        public MainViewModel() : this(new ProcessService())
        {
        }

        public MainViewModel(IProcessService service)
        {
            _service = service;

            for (int i = 0; i < Environment.ProcessorCount; i++)
                Cores.Add(new CpuCore(i));
        }

        public async Task RefreshAsync()
        {
            var processInfoList = await Task.Run(() => _service.GetProcesses());

            var uiList = processInfoList.Select(p => new ProcessTreeNode
            {
                Id = p.Id,
                Name = p.Name,
                Memory = p.MemoryUsage,
                Priority = p.Priority,
                CpuAffinity = AffinityHelper.MaskToString(
                    p.AffinityMask.ToInt64(),
                    Environment.ProcessorCount),
                Threads = p.ThreadCount,
                CpuTime = p.CpuTime,
            }).ToList();

            SyncCollection(uiList);
        }

        public void SyncCollection(List<ProcessTreeNode> newList)
        {
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
                    existing.Priority = proc.Priority;
                    existing.CpuAffinity = proc.CpuAffinity;
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

        public async void ApplyAffinity()
        {
            if (SelectedProcess == null) return;

            var maskLong = AffinityHelper.BuildMask(
                Cores.Select(c => c.IsEnabled).ToArray());

            var maskPtr = new IntPtr(maskLong);

            _service.SetAffinity(SelectedProcess.Id, maskPtr);

            await RefreshAsync();
        }

        private void LoadAffinity()
        {
            if (SelectedProcess == null) return;

            var mask = _service.GetAffinity(SelectedProcess.Id);

            foreach (var core in Cores)
                core.IsEnabled = AffinityHelper.IsCoreEnabled(mask, core.Index);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void LoadThreads()
        {
            Threads.Clear();

            if (SelectedProcess == null)
                return;

            List<ThreadInfo> list;

            try
            {
                list = _service.GetThreads(SelectedProcess.Id);
            }
            catch
            {
                return;
            }

            foreach (var t in list)
                Threads.Add(t);
        }
    }
}