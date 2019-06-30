using System.Threading.Tasks;
using HuntLog.Services;
using HuntLog.AppModule.Hunters;
using HuntLog.AppModule.Hunts;
using HuntLog.InputViews;
using LightInject;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using HuntLog.AppModule.Logs;
using HuntLog.AppModule.Dogs;
using HuntLog.AppModule.Species;
using HuntLog.AppModule.CustomFields;
using HuntLog.AppModule.Stats;
using HuntLog.AppModule.Setup;
using HuntLog.AppModule.Stats.Pages;
using System.Reflection;
using System.Linq;
using System;
using HuntLog.Factories;

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
            InitializeData(fileManager);

            App.Navigator = container.GetInstance<INavigator>();

            await ConfigureApplication(container);
        }

        protected void RegisterViews(INavigator viewFactory)    
        {
            var vms = AssemblyFactory.GetViewModels();
            var views = AssemblyFactory.GetViews();

            foreach (var vm in vms)
            {
                var view = views.SingleOrDefault(x => x.Name.Replace("View", "ViewModel") == vm.Name);
                if(view != null)
                {
                    viewFactory.Register(vm, view);
                }
            }
        }

        protected async Task ConfigureApplication(IServiceFactory container)
        {
            var tabbed = new Xamarin.Forms.TabbedPage();
            tabbed.On<Xamarin.Forms.PlatformConfiguration.Android>().SetToolbarPlacement(ToolbarPlacement.Bottom);

            tabbed.Children.Add(CreateTab(container.GetInstance<HuntsView>(), "Jaktloggen", "Tabbar/gevir.png"));
            tabbed.Children.Add(CreateTab(container.GetInstance<SetupView>(), "Oppsett", "Tabbar/hunters.png"));
            tabbed.Children.Add(CreateTab(container.GetInstance<StatsView>(), "Statistikk", "Tabbar/stats.png"));
            tabbed.Children.Add(CreateTab(new Page(), "Info", "Tabbar/info.png"));

            _application.MainPage = tabbed;
            await Task.CompletedTask;
        }

        private static NavigationPage CreateTab(Page page, string title, string icon = null)
        {
            var huntPage = new NavigationPage(page);
            huntPage.Title = title;
            huntPage.IconImageSource = icon;
            return huntPage;
        }

        private void InitializeData(IFileManager fileManager)
        {

            if (!fileManager.Exists("arter.xml")) 
            {
                fileManager.CopyToAppFolder("arter.xml");
                fileManager.CopyToAppFolder("artgroup.xml");

                fileManager.CopyToAppFolder("loggtypegroup.xml");
                fileManager.CopyToAppFolder("loggtyper.xml");
            }

//#if DEBUG
            //if (!fileManager.Exists("jakt.xml"))
            //{
            //    fileManager.CopyToAppFolder("dogs.xml");
            //    fileManager.CopyToAppFolder("jakt.xml");
            //    fileManager.CopyToAppFolder("jegere.xml");
            //    fileManager.CopyToAppFolder("logger.xml");
            //    fileManager.CopyToAppFolder("myspecies.xml");
            //    fileManager.CopyToAppFolder("selectedartids.xml");
            //}
//#endif
        }
    }
}
