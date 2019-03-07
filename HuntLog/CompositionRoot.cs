using System;
using LightInject;
using HuntLog.Services;

using Xamarin.Forms;
using HuntLog.Interfaces;
using HuntLog.AppModule.Hunters;
using HuntLog.Factories;
using HuntLog.AppModule.InputViews;
using HuntLog.AppModule.Hunts;

namespace HuntLog
{
    public class CompositionRoot : ICompositionRoot
    {       
        public void Compose(IServiceRegistry serviceRegistry)
        {

            serviceRegistry
            .RegisterSingleton<INavigator>(f => new Navigator(f.GetInstance<Lazy<TabbedPage>>(), f))
            .RegisterSingleton<TabbedPage>(f => ((TabbedPage)Application.Current.MainPage))
            .RegisterSingleton<IFileUtility>(f => DependencyService.Get<IFileUtility>())
            .RegisterSingleton<IFileManager, FileManager>()
            .RegisterSingleton<IMediaService, MediaService>()
            .RegisterSingleton<IHuntService, HuntService>()
            .RegisterSingleton<IHunterService, HunterService>()
            .RegisterSingleton<IDialogService, DialogService>()
            .RegisterSingleton<IHunterFactory, HunterFactory>()
            .Register<HuntViewModel>()
            .Register<EditHuntViewModel>()
            .Register<HuntListItemViewModel>()
            .Register<HuntsViewModel, HuntsViewModel>(new PerContainerLifetime())
            .Register<HuntView>()
            .Register<EditHuntView>()
            .Register<HuntsView>()

            .Register<HuntersViewModel>()
            .Register<HunterViewModel>()
            .Register<HuntersView>()
            .Register<HunterView>()

            .Register<InputImageViewModel>()
            .Register<InputImageView>();

        }
    }
}





