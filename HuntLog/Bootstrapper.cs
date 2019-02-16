using System.Threading.Tasks;
using HuntLog.Services;
using HuntLog.ViewModels.Hunters;
using HuntLog.ViewModels.Hunts;
using HuntLog.Views.Hunters;
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
            viewFactory.Register<HuntersViewModel, HuntersView>();
            viewFactory.Register<HunterViewModel, HunterView>();

        }

        protected async Task ConfigureApplication(IServiceFactory container)
        {
            var tabbed = new TabbedPage();
            var huntPage = CreateTab(container.GetInstance<HuntsView>(), "Jaktloggen");
            var hunterPage = CreateTab(container.GetInstance<HuntersView>(), "Jegere");

            tabbed.Children.Add(huntPage);
            tabbed.Children.Add(hunterPage);

            _application.MainPage = tabbed;
            await Task.CompletedTask;
        }

        private static NavigationPage CreateTab(Page page, string title, string icon = null)
        {
            var huntPage = new NavigationPage(page);
            huntPage.Title = title;
            huntPage.Icon = icon;
            return huntPage;
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
