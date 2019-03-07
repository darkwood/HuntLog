using Xamarin.Forms;

namespace HuntLog.AppModule.Hunters
{
    public partial class HuntersView : ContentPage
    {
        private readonly HuntersViewModel _viewModel;

        public HuntersView(HuntersViewModel viewModel)
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
