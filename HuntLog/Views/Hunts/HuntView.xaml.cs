using HuntLog.ViewModels.Hunts;
using Xamarin.Forms;

namespace HuntLog.Views.Hunts
{
    public partial class HuntView : ContentPage
    {
        private readonly HuntViewModel _viewModel;

        public HuntView(HuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }

        protected async override void OnAppearing()
        {
            await _viewModel.OnAppearing();
            base.OnAppearing();
        }

    }
}
