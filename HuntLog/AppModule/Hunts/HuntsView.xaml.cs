using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public partial class HuntsView : ContentPage
    {
        private readonly HuntsViewModel _viewModel;

        public HuntsView(HuntsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.InitializeAsync();
            base.OnAppearing();
        }
    }
}
