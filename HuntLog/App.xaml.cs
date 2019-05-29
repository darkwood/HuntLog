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
#if DEBUG
            HotReloader.Current.Start(this);
#endif
            InitializeComponent();
        }

        protected async override void OnStart()
        {
            AppCenter.Start("ios=d512c099-5374-4e83-a058-40d4582b1b79;" +
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
