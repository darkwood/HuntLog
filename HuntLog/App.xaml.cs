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
        }

        protected async override void OnStart()
        {
            var bootstrapper = new Bootstrapper(this);
            await bootstrapper.Run();
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
