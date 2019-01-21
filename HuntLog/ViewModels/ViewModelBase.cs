using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HuntLog.ViewModels;

namespace HuntLog.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Title { get; set; }
        bool IsBusy { get; set; }
        Task InitializeAsync(object initData);
    }

    public abstract class ViewModelBase : IViewModel
    {
        private string _title;
        public virtual string Title
        {
            get => _title;
            set
            {
                SetProperty(ref _title, value);
            }
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy; 
            set
            {
                SetProperty(ref _isBusy, value);
            }
        }


        public virtual async Task InitializeAsync(object initData = null) { await Task.CompletedTask; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);

            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
