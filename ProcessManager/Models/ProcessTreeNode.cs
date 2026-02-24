using System.ComponentModel;

namespace ProcessManager.Models
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

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string n) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}