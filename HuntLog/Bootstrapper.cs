using System.Threading.Tasks;
using HuntLog.Services;
using HuntLog.AppModule.Hunters;
using HuntLog.AppModule.Hunts;
using HuntLog.InputViews;
using LightInject;
using Xamarin.Forms;
using HuntLog.AppModule.Logs;
using HuntLog.AppModule.Dogs;
using HuntLog.AppModule.Species;

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

            App.Navigator = container.GetInstance<INavigator>();

            await ConfigureApplication(container);
        }

        protected void RegisterViews(INavigator viewFactory)
        {
            viewFactory.Register<HuntsViewModel, HuntsView>();
            viewFactory.Register<HuntViewModel, HuntView>();
            viewFactory.Register<EditHuntViewModel, EditHuntView>();
            viewFactory.Register<LogViewModel, LogView>();
            viewFactory.Register<HuntersViewModel, HuntersView>();
            viewFactory.Register<HunterViewModel, HunterView>();
            viewFactory.Register<DogsViewModel, DogsView>();
            viewFactory.Register<DogViewModel, DogView>();
            viewFactory.Register<SpeciesViewModel, SpeciesView>();
            viewFactory.Register<SpecieViewModel, SpecieView>();
            viewFactory.Register<InputImageViewModel, InputImageView>();
            viewFactory.Register<InputPositionViewModel, InputPositionView>();
            viewFactory.Register<InputDateViewModel, InputDateView>();
            viewFactory.Register<InputPickerViewModel, InputPickerView>();
        }

        protected async Task ConfigureApplication(IServiceFactory container)
        {
            var tabbed = new TabbedPage();

            tabbed.Children.Add(CreateTab(container.GetInstance<HuntsView>(), "Jaktloggen", "Tabbar/gevir.png"));
            tabbed.Children.Add(CreateTab(container.GetInstance<HuntersView>(), "Jegere", "Tabbar/hunters.png"));
            tabbed.Children.Add(CreateTab(container.GetInstance<DogsView>(), "Hunder", "Tabbar/dog.png"));
            tabbed.Children.Add(CreateTab(container.GetInstance<SpeciesView>(), "Arter", "Tabbar/Arter.png"));
            tabbed.Children.Add(CreateTab(new Page(), "Statistikk", "Tabbar/stats.png"));
            tabbed.Children.Add(CreateTab(new Page(), "Info", "Tabbar/info.png"));

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
