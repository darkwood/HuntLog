using System;
using System.Threading.Tasks;
using HuntLog.ViewModels;
using Xamarin.Forms;

namespace HuntLog.Services
{
    public interface INavigator
    {
        Task<IViewModel> PopAsync();
        Task<IViewModel> PopModalAsync();
        Task PopToRootAsync();
        Task<TViewModel> PushModalAsync<TViewModel>(Func<TViewModel, Task> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null) where TViewModel : class, IViewModel;
        Task<TViewModel> PushAsync<TViewModel>(Func<TViewModel, Task> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null) where TViewModel : class, IViewModel;
        void Register<TViewModel, TView>() where TViewModel : class, IViewModel where TView : Page;
    }
}
