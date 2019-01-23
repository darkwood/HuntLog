using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HuntLog.ViewModels;
using LightInject;
using Xamarin.Forms;

namespace HuntLog.Services
{

    public class Navigator : INavigator
    {
        private readonly Lazy<INavigation> _navigation;
        private readonly IServiceFactory serviceFactory;
        private readonly IDictionary<Type, Type> _map = new Dictionary<Type, Type>();


        public Navigator(Lazy<INavigation> navigation, IServiceFactory serviceFactory)
        {
            _navigation = navigation;
            this.serviceFactory = serviceFactory;
        }

        public void Register<TViewModel, TView>()
            where TViewModel : class, IViewModel
            where TView : Page
        {
            _map[typeof(TViewModel)] = typeof(TView);
        }


        private INavigation Navigation
        {
            get { return _navigation.Value; }
        }

        public async Task<IViewModel> PopAsync()
        {
            Page view = await Navigation.PopAsync();
            return view.BindingContext as IViewModel;
        }

        public async Task<IViewModel> PopModalAsync()
        {
            var view = await Navigation.PopAsync();
            return view.BindingContext as IViewModel;
        }

        public async Task PopToRootAsync()
        {
            await Navigation.PopToRootAsync();
        }

        public async Task<TViewModel> PushAsync<TViewModel>(Func<TViewModel, Task> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null) where TViewModel : class, IViewModel
        {
            var view = (Page)serviceFactory.GetInstance(_map[typeof(TViewModel)]);
            var viewModel = (TViewModel)view.BindingContext;

            if (beforeNavigate != null)
            {
                await beforeNavigate(viewModel);
            }

            await Navigation.PushAsync(view);

            if (afterNavigate != null)
            {
                await afterNavigate(viewModel);
            }

            return viewModel;
        }

        public async Task<TViewModel> PushModalAsync<TViewModel>(TViewModel viewModel, Func<TViewModel, Task> beforeNavigate = null, Func<TViewModel, Task> afterNavigate = null)
            where TViewModel : class, IViewModel
        {
            var view = (Page)serviceFactory.GetInstance(_map[typeof(TViewModel)]);
            view.BindingContext = viewModel;

            if (beforeNavigate != null)
            {
                await beforeNavigate(viewModel);
            }

            await Navigation.PushModalAsync(view);

            if (afterNavigate != null)
            {
                await afterNavigate(viewModel);
            }

            return viewModel;
        }
    }
}
