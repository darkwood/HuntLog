using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public partial class EditHuntView : ContentPage
    {
        private readonly EditHuntViewModel _viewModel;

        public EditHuntView(EditHuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }
    }
}
