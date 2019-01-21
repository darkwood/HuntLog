using System;
using Autofac;
using HuntLog.Factories;
using HuntLog.Services;
using HuntLog.ViewModels.Hunts;
using HuntLog.Views.Hunts;
using Xamarin.Forms;

namespace HuntLog
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ViewFactory>().As<IViewFactory>().SingleInstance();
            builder.RegisterType<Navigator>().As<INavigator>().SingleInstance();
            builder.Register<INavigation>(context => Application.Current.MainPage.Navigation).SingleInstance();
            builder.RegisterType<HuntService>().As<IHuntService>().SingleInstance();


            //ViewModels
            builder.RegisterType<HuntViewModel>();
            builder.RegisterType<HuntListItemViewModel>();
            builder.RegisterType<HuntsViewModel>().SingleInstance();

            //Views
            builder.RegisterType<HuntView>();
            builder.RegisterType<HuntsView>().SingleInstance();

            base.Load(builder);
        }
    }
}
