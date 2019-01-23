using HuntLog.Services;
using HuntLog.ViewModels.Hunts;
using HuntLog.Views.Hunts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HuntLog
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeDummyData();
        }

        protected async override void OnStart()
        {
            var bootstrapper = new Bootstrapper(this);
            await bootstrapper.Run();
        }

        private void InitializeDummyData()
        {
            if (!FileManager.Exists("jakt.xml"))
            {
                FileManager.CopyToAppFolder("arter.xml");
                FileManager.CopyToAppFolder("artgroup.xml");
                FileManager.CopyToAppFolder("dogs.xml");
                FileManager.CopyToAppFolder("jakt.xml");
                FileManager.CopyToAppFolder("jegere.xml");
                FileManager.CopyToAppFolder("logger.xml");
                FileManager.CopyToAppFolder("loggtypegroup.xml");
                FileManager.CopyToAppFolder("loggtyper.xml");
                FileManager.CopyToAppFolder("myspecies.xml");
                FileManager.CopyToAppFolder("selectedartids.xml");
            }
        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
