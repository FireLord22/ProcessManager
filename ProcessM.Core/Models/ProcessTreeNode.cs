using System.ComponentModel;
using System.Diagnostics;

namespace ProcessM.Core.Models
{
    public class ProcessTreeNode : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Name { get; set; }

        private long _memory;
        public long Memory
        {
            get => _memory;
            set
            {
                _memory = value;
                OnPropertyChanged(nameof(Memory));
            }
        }

        private ProcessPriorityClass _priority;
        public ProcessPriorityClass Priority
        {
            get => _priority;
            set
            {
                _priority = value;
                OnPropertyChanged(nameof(Priority));
            }
        }

        private string _cpuAffinity;
        public string CpuAffinity
        {
            get => _cpuAffinity;
            set
            {
                _cpuAffinity = value;
                OnPropertyChanged(nameof(CpuAffinity));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string n) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}