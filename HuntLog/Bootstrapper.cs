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
            IFileManager fileManager = container.GetInstance<IFileManager>();
            InitializeDummyData(fileManager);

            await ConfigureApplication(container);
        }

        protected void RegisterViews(INavigator viewFactory)
        {
            viewFactory.Register<HuntsViewModel, HuntsView>();
            viewFactory.Register<HuntViewModel, HuntView>();
            viewFactory.Register<EditHuntViewModel, EditHuntView>();

        }

        protected async Task ConfigureApplication(IServiceFactory container)
        {
            var mainPage = container.GetInstance<HuntsView>();
            var huntsViewModel = (HuntsViewModel) mainPage.BindingContext;

            var navigationPage = new NavigationPage(mainPage); //TODO: Register NavigationPage in container?
            _application.MainPage = navigationPage;
            await Task.CompletedTask;
        }

        private void InitializeDummyData(IFileManager fileManager)
        {

            if (!fileManager.Exists("jakt.xml"))
            {
                fileManager.CopyToAppFolder("arter.xml");
                fileManager.CopyToAppFolder("artgroup.xml");
                fileManager.CopyToAppFolder("dogs.xml");
                fileManager.CopyToAppFolder("jakt.xml");
                fileManager.CopyToAppFolder("jegere.xml");
                fileManager.CopyToAppFolder("logger.xml");
                fileManager.CopyToAppFolder("loggtypegroup.xml");
                fileManager.CopyToAppFolder("loggtyper.xml");
                fileManager.CopyToAppFolder("myspecies.xml");
                fileManager.CopyToAppFolder("selectedartids.xml");
            }
        }
    }
}
