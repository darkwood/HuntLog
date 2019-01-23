using System;
using LightInject;
using HuntLog.Services;
using HuntLog.ViewModels.Hunts;
using HuntLog.Views.Hunts;
using Xamarin.Forms;

namespace HuntLog
{
    public class CompositionRoot : ICompositionRoot
    {       
        public void Compose(IServiceRegistry serviceRegistry)
        {

                serviceRegistry.RegisterSingleton<INavigator>(f => new Navigator(f.GetInstance<Lazy<INavigation>>(), f))
                .RegisterSingleton<INavigation>(f => Application.Current.MainPage.Navigation)
                .RegisterSingleton<IHuntService, HuntService>()
                .Register<HuntViewModel>()
                .Register<HuntListItemViewModel>()
                .Register<HuntsViewModel, HuntsViewModel>(new PerContainerLifetime())
                .Register<HuntView>()
                .Register<HuntsView>();
        }
    }
}





