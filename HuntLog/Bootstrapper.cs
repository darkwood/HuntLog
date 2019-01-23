using System.Threading.Tasks;
using HuntLog.Services;
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

        public async Task Run()
        {
            var containerOptions = new ContainerOptions() { EnablePropertyInjection = false };
            var container = new ServiceContainer(containerOptions);
            container.RegisterFrom<CompositionRoot>();

            RegisterViews(container.GetInstance<INavigator>());

            await ConfigureApplication(container);
        }

        protected void RegisterViews(INavigator viewFactory)
        {
            viewFactory.Register<HuntsViewModel, HuntsView>();
            viewFactory.Register<HuntViewModel, HuntView>();
        }

        protected async Task ConfigureApplication(IServiceFactory container)
        {
            var huntsViewModel = container.GetInstance<HuntsViewModel>();
            var mainPage = new HuntsView(huntsViewModel);
            var navigationPage = new NavigationPage(mainPage);
            _application.MainPage = navigationPage;
            await huntsViewModel.InitializeAsync();
        }
    }
}
