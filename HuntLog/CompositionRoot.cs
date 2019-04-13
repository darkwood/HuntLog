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
using HuntLog.AppModule.Dogs;
using HuntLog.AppModule.Species;

namespace HuntLog
{
    public class CompositionRoot : ICompositionRoot
    {       
        public void Compose(IServiceRegistry serviceRegistry)
        {

            _ = serviceRegistry
            .RegisterSingleton<INavigator>(f => new Navigator(f.GetInstance<Lazy<TabbedPage>>(), f))
            .RegisterSingleton<TabbedPage>(f => ((TabbedPage)Application.Current.MainPage))
            .RegisterSingleton<IFileUtility>(f => DependencyService.Get<IFileUtility>())
            .RegisterSingleton<IFileManager, FileManager>()
            .RegisterSingleton<IMediaService, MediaService>()
            .RegisterSingleton<IBaseService<Jakt>, BaseService<Jakt>>()
            .RegisterSingleton<IBaseService<Jeger>, BaseService<Jeger>>()
            .RegisterSingleton<IBaseService<Logg>, BaseService<Logg>>()
            .RegisterSingleton<IBaseService<Dog>, BaseService<Dog>>()
            .RegisterSingleton<IBaseService<Art>, BaseService<Art>>()
            .RegisterSingleton<ISelectService<Art>, SelectService<Art>>()
            .RegisterSingleton<IBaseService<ArtGroup>, BaseService<ArtGroup>>()
            .RegisterSingleton<IDialogService, DialogService>()
            .RegisterSingleton<IHuntFactory, HuntFactory>()

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

            .Register<DogsViewModel>()
            .Register<DogsView>()

            .Register<DogViewModel>()
            .Register<DogView>()

            .Register<SpeciesViewModel>()
            .Register<SpeciesView>()

            .Register<SpecieViewModel>()
            .Register<SpecieView>()

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





