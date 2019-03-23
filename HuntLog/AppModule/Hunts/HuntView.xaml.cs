using System;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts
{
    public partial class HuntView : ContentPage
    {
        private readonly HuntViewModel _viewModel;

        public HuntView()
        {
            BindingContext = new HuntViewModel(null, null, null);
        }
        public HuntView(HuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }
    }

    public class HuntViewModel : HuntViewModelBase
    {
        private readonly IHuntService _huntService;
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;
        private Jakt _dto;

        public Command EditCommand { get; set; }
        public HuntViewModel(IHuntService huntService, IHunterService hunterService, INavigator navigator)
        {
            _huntService = huntService;
            _hunterService = hunterService;
            _navigator = navigator;
            EditCommand = new Command(async () => await EditItem());
        }
        private async Task EditItem()
        {
            Action<Jakt> callback = (arg) => { SetState(arg); };

            await _navigator.PushAsync<EditHuntViewModel>(
                    beforeNavigate: (arg) => arg.SetState(_dto, callback),
                    afterNavigate: async (arg) => await arg.AfterNavigate());
        }

        public void SetState(Jakt dto)
        {
            _dto = dto;
            SetStateFromDto(_dto);
        }
    }
}
