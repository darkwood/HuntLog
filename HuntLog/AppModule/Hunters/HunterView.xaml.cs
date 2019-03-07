using Xamarin.Forms;

namespace HuntLog.AppModule.Hunters
{
    public partial class HunterView : ContentPage
    {
        private readonly HunterViewModel _viewModel;

        public HunterView(HunterViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
