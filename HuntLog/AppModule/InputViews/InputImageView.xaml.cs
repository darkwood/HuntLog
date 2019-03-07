using Xamarin.Forms;

namespace HuntLog.AppModule.InputViews
{
    public partial class InputImageView : ContentPage
    {
        private readonly InputImageViewModel _viewModel;

        public InputImageView(InputImageViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
