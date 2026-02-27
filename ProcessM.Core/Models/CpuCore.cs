using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ProcessM.Core.Models
{
    public class CpuCore : INotifyPropertyChanged
    {
        public int Index { get; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public CpuCore(int index)
        {
            Index = index;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}