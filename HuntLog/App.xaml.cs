using HuntLog.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
//[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HuntLog
{
    public partial class App : Application
    {
        public static INavigator Navigator { get; set; }
        public App()
        {
            InitializeComponent();
#if DEBUG
            HotReloader.Current.Run(this);
#endif
        }

        protected async override void OnStart()
        {
            AppCenter.Start("ios=c7df6e06-24c2-46dd-87da-5cf7f6d7858a;" +
                  "uwp={Your UWP App secret here};" +
                  "android={Your Android App secret here}",
                  typeof(Analytics), typeof(Crashes));

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
