using System;
using LightInject;
using HuntLog.Services;

using Xamarin.Forms;
using HuntLog.Interfaces;
using HuntLog.AppModule.Hunters;
using HuntLog.Factories;
using HuntLog.InputViews;
using HuntLog.AppModule.Hunts;
using HuntLog.AppModule.Logs;
using HuntLog.Models;
using HuntLog.Cells;

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
            .RegisterSingleton<IBaseService<Jakt>, BaseService<Jakt>>()
            .RegisterSingleton<IBaseService<Jeger>, BaseService<Jeger>>()
            .RegisterSingleton<IBaseService<Logg>, BaseService<Logg>>()
            .RegisterSingleton<IDialogService, DialogService>()
            .RegisterSingleton<IHunterFactory, HunterFactory>()

            .Register<HuntsViewModel, HuntsViewModel>(new PerContainerLifetime())
            .Register<HuntsView>()
            .Register<HuntListItemViewModel>()

            .Register<HuntViewModel>()
            .Register<HuntView>()

            .Register<EditHuntViewModel>()
            .Register<EditHuntView>()

            .Register<LogViewModel>()
            .Register<LogView>()

            .Register<HuntersViewModel>()
            .Register<HuntersView>()

            .Register<HunterViewModel>()
            .Register<HunterView>()

            .Register<InputImageViewModel>()
            .Register<InputImageView>()

            .Register<InputPositionViewModel>()
            .Register<InputPositionView>()

            .Register<InputDateViewModel>()
            .Register<InputDateView>()

            .Register<InputPickerViewModel>()
            .Register<InputPickerView>();

        }
    }
}





