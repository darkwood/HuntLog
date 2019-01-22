using System;
using Autofac;
using HuntLog.Factories;
using HuntLog.ViewModels.Hunts;
using HuntLog.Views.Hunts;
using LightInject;
using Xamarin.Forms;

namespace HuntLog
{
    public class Bootstrapper
    {
        private readonly App _application;

        public Bootstrapper(App application)
        {
            _application = application;
        }

        public void Run()
        {
            var containerOptions = new ContainerOptions() { EnablePropertyInjection = false };
            var container = new ServiceContainer(containerOptions);
            container.RegisterFrom<CompositionRoot>();


            var builder = new ContainerBuilder();
            var viewFactory = container.GetInstance<IViewFactory>();


            RegisterViews(viewFactory);

            ConfigureApplication(container);
        }

        protected void RegisterViews(IViewFactory viewFactory)
        {
            viewFactory.Register<HuntsViewModel, HuntsView>();
            viewFactory.Register<HuntViewModel, HuntView>();
        }

        protected void ConfigureApplication(IServiceFactory container)
        {
            // set main page
            var viewFactory = container.GetInstance<IViewFactory>();
            var mainPage = viewFactory.Resolve<HuntsViewModel>();
            var navigationPage = new NavigationPage(mainPage);

            _application.MainPage = navigationPage;
        }
    }
}
