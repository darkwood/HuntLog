using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.AppModule;
using LightInject;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HuntLog.Services
{
    public interface INavigator
    {
        Task<IViewModel> PopAsync();
        Task<IViewModel> PopModalAsync();
        Task PopToRootAsync(bool animate = true);
        Task<TViewModel> PushModalAsync<TViewModel>(Action<TViewModel>beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null) where TViewModel : class, IViewModel;
        Task<TViewModel> PushAsync<TViewModel>(Action<TViewModel> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null, bool animated = true) where TViewModel : class, IViewModel;
        void Register<TViewModel, TView>() where TViewModel : class, IViewModel where TView : Page;
        void RegisterView<TViewModel, TView>() where TViewModel : class, IViewModel where TView : ContentView;
        void Register(Type vm, Type view);
    }

    public class Navigator : INavigator
    {
        private readonly Lazy<TabbedPage> _tabbedPage;
        private readonly IServiceFactory serviceFactory;
        private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();


        public Navigator(Lazy<TabbedPage> tabbedPage, IServiceFactory serviceFactory)
        {
            _tabbedPage = tabbedPage;
            this.serviceFactory = serviceFactory;
        }

        public void Register(Type vm, Type view)
        {
            _map[vm] = view;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : Page
        {
            _map[typeof(TViewModel)] = typeof(TView);
        }

        public void RegisterView<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : ContentView
        {
            _map[typeof(TViewModel)] = typeof(TView);
        }


        private INavigation Navigation
        {
            get { return _tabbedPage.Value.CurrentPage.Navigation; }
        }

        public async Task<IViewModel> PopAsync()
        {
            Page view = await Navigation.PopAsync();
            return view.BindingContext as IViewModel;
        }

        public async Task<IViewModel> PopModalAsync()
        {
            var view = await Navigation.PopModalAsync();
            return view.BindingContext as IViewModel;
        }

        public async Task PopToRootAsync(bool animated = true)
        {
            await Navigation.PopToRootAsync(animated);
        }

        public async Task<TViewModel> PushAsync<TViewModel>(Action<TViewModel> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null, bool animated = true) where TViewModel : class, IViewModel
        {
            var view = (Page)serviceFactory.GetInstance(_map[typeof(TViewModel)]);
            var viewModel = (TViewModel)view.BindingContext;

            beforeNavigate?.Invoke(viewModel);


            if (afterNavigate != null)
            {
                await afterNavigate(viewModel);
            }

            await Navigation.PushAsync(view, animated);

            return viewModel;
        }

        public async Task<TViewModel> PushModalAsync<TViewModel>(Action<TViewModel> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null) where TViewModel : class, IViewModel 
        {
            var view = (Page)serviceFactory.GetInstance(_map[typeof(TViewModel)]);
            var viewModel = (TViewModel)view.BindingContext;

            beforeNavigate?.Invoke(viewModel);

            await Navigation.PushModalAsync(new NavigationPage(view));

            if (afterNavigate != null)
            {
                await afterNavigate(viewModel);
            }

            return viewModel;
        }

    }
}
