using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
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
    }
}
