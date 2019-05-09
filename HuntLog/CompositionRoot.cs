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
using HuntLog.AppModule.CustomFields;
using HuntLog.AppModule.Stats;
using System.Reflection;
using System.Linq;

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
            .RegisterSingleton<IBaseService<LoggType>, BaseService<LoggType>>()
            .RegisterSingleton<ISelectService<LoggType>, SelectService<LoggType>>()
            .RegisterSingleton<IBaseService<LoggTypeGroup>, BaseService<LoggTypeGroup>>()
            .RegisterSingleton<IDialogService, DialogService>()
            .RegisterSingleton<IHuntFactory, HuntFactory>()
            .RegisterSingleton<ICustomFieldFactory, CustomFieldFactory>()


            .Register<HuntsViewModel, HuntsViewModel>(new PerContainerLifetime())
            .Register<HuntsView>()
            .Register<HuntListItemViewModel>()

            .Register<HuntViewModel>()
            .Register<HuntView>()

            .Register<EditHuntViewModel>()
            .Register<EditHuntView>()

            .Register<LogViewModel>()
            .Register<LogView>()
            .Register<LogListItemViewModel>()
            .Register<LogViewCode>()

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

            .Register<CustomFieldsViewModel>(new PerContainerLifetime())
            .Register<CustomFieldsView>()

            .Register<CustomFieldViewModel>()
            .Register<CustomFieldView>()

            //.Register<LogCustomFieldsViewModel>()
            //.Register<LogCustomFieldsView>()

            .Register<InputImageViewModel>()
            .Register<InputImageView>()

            .Register<InputPositionViewModel>()
            .Register<InputPositionView>()

            .Register<InputDateViewModel>()
            .Register<InputDateView>()

            .Register<InputTimeViewModel>()
            .Register<InputTimeView>()

            .Register<InputPickerViewModel>()
            .Register<InputPickerView>()

            .Register<InputTextViewModel>()
            .Register<InputTextView>()

            .Register<StatsViewModel>()
            .Register<StatsView>()

            .Register<StatsMapViewModel>()
            .Register<StatsMapView>()

            .RegisterSingleton<StatsFilterViewModel>()
            .RegisterSingleton<StatsFilterView>()

            ;
        }
    }
}





