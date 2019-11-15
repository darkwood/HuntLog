using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HuntLog.Services;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.AppModule
{
    public interface IViewModel : INotifyPropertyChanged
    {
        string Title { get; set; }
        bool IsBusy { get; set; }
        MediaFile MediaFile { get; set; }
    }

    public abstract class ViewModelBase : IViewModel
    {
        public string ID { get; set; }
        public ImageSource ImageSource { get; set; }
        public bool IsNew => string.IsNullOrEmpty(ID);
        public MediaFile MediaFile { get; set; }
        public string Title { get; set; }
        public bool IsBusy { get; set; }

        public string SaveImage(string filename, IFileManager fileManager)
        {
            using (var memoryStream = new MemoryStream())
            {
                MediaFile.GetStreamWithImageRotatedForExternalStorage().CopyTo(memoryStream);
                MediaFile.Dispose();
                fileManager.SaveImage(filename, memoryStream.ToArray());
                return filename;
            }
        }

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
