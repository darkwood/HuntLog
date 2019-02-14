using System.Linq;
using HuntLog.ViewModels.Hunts;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Views.Hunts
{
    public partial class HuntView : ContentPage
    {
        public HuntView()
        {
            InitializeComponent();
        }
        private readonly HuntViewModel _viewModel;

        public HuntView(HuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.SetMap(map);
        }
    }
}
