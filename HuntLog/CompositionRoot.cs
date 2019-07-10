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
using HuntLog.AppModule.Setup;
using HuntLog.AppModule.Stats.Pages;
using HuntLog.AppModule;
using System.Timers;

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
            ;

            foreach (var vm in AssemblyFactory.GetViewModels())
            {
                serviceRegistry.Register(vm);
            }
            foreach (var view in AssemblyFactory.GetViews())
            {
                serviceRegistry.Register(view);
            }

            serviceRegistry.RegisterSingleton<StatsFilterViewModel>();
            serviceRegistry.RegisterSingleton<StatsFilterView>();

        }
    }
}





