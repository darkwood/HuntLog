using System;
using System.Globalization;
using System.Threading.Tasks;
using HuntLog.Factories;
using HuntLog.Helpers;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Logs
{
    public class LogListItemViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IHuntFactory _huntFactory;
        private readonly IBaseService<Logg> _logService;
        private readonly IBaseService<Jakt> _huntService;
        private Logg _dto;
        private Jakt _huntDto;

        public string Detail { get; set; }
        public Command ItemTappedCommand { get; set; }
        public DateTime Date { get;  set; }
        public Art Specie { get; set; }
        public Jeger Hunter { get; set; }
        public Dog Dog { get; set; }

        public LogListItemViewModel(INavigator navigator, 
                                    IHuntFactory huntFactory,
                                    IBaseService<Logg> logService, 
                                    IBaseService<Jakt> huntService)
        {
            _navigator = navigator;
            _huntFactory = huntFactory;
            _logService = logService;
            _huntService = huntService;

            ItemTappedCommand = new Command(async () => { await ShowItem(); });
        }

        private async Task ShowItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                beforeNavigate: (arg) => arg.BeforeNavigate(_dto, _huntDto));
                //afterNavigate: async (arg) => await arg.OnAppearing());
        }

        public void BeforeNavigate(Logg dto)
        {
            _dto = dto;
            Date = _dto.Dato;
            Title = CreateTitle();
            ImageSource = Utility.GetImageSource($"jaktlogg_{ID}.jpg");
        }

        private string CreateTitle()
        {
            
            return $"Kl. {Date.ToString("hh:mm", new CultureInfo("nb-no"))}";
        }

        public async Task OnAppearing()
        {
            _huntDto = await _huntService.Get(_dto.JaktId);
            Detail = await _huntFactory.CreateLogSummary(_dto);
        }
    }
}

