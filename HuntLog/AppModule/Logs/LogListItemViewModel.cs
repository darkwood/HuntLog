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
        private readonly IBaseService<Logg> _logService;
        private readonly IBaseService<Art> _specieService;
        private readonly IBaseService<Jeger> _hunterService;
        private readonly IBaseService<Dog> _dogService;
        private Logg _dto;

        public string Detail { get; set; }
        public Command ItemTappedCommand { get; set; }
        public DateTime Date { get;  set; }
        public Art Specie { get; set; }
        public Jeger Hunter { get; set; }
        public Dog Dog { get; set; }

        public LogListItemViewModel(INavigator navigator, 
                                    IBaseService<Logg> logService, 
                                    IBaseService<Art> specieService,
                                    IBaseService<Jeger> hunterService,
                                    IBaseService<Dog> dogService)
        {
            _navigator = navigator;
            _logService = logService;
            _specieService = specieService;
            _hunterService = hunterService;
            _dogService = dogService;
            ItemTappedCommand = new Command(async () => { await ShowItem(); });
        }

        private async Task ShowItem()
        {
            await _navigator.PushAsync<LogViewModel>(
                beforeNavigate: (arg) => arg.BeforeNavigate(_dto),
                afterNavigate: async (arg) => await arg.AfterNavigate());
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

        public override async Task AfterNavigate()
        {
            if (!string.IsNullOrEmpty(_dto.ArtId))
            {
                Specie = await _specieService.Get(_dto.ArtId);
            }

            if (!string.IsNullOrEmpty(_dto.JegerId))
            {
                Hunter = await _hunterService.Get(_dto.JegerId);
            }

            if (!string.IsNullOrEmpty(_dto.DogId))
            {
                Dog = await _dogService.Get(_dto.DogId);
            }

            Detail = $"{_dto.Sett} Sett ({Specie?.Navn}), {_dto.Skudd} skudd og {_dto.Treff} treff";
        }
    }
}

