using System;
using LightInject;
using HuntLog.Factories;
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
            serviceRegistry.RegisterSingleton<IViewFactory>(f => new ViewFactory(f))
                .RegisterSingleton<INavigator, Navigator>()
                .RegisterSingleton<INavigation>(f => Application.Current.MainPage.Navigation)
                .RegisterSingleton<IHuntService, HuntService>()
                .Register<HuntViewModel>()
                .Register<HuntListItemViewModel>()
                .Register<HuntsViewModel>()
                .Register<HuntView>()
                .Register<HuntsView>();
        }
    }
}





