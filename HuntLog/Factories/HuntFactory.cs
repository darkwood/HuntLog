using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.Species;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.Factories
{
    public interface IHuntFactory
    {
        Task<List<PickerItem>> CreateHunterPickerItems(List<string> selectedIds, string huntId = null);
        Task<List<PickerItem>> CreateDogPickerItems(List<string> selectedIds, string huntId = null);
        Task<List<PickerItem>> CreateSpeciePickerItems(string selectedId);
        Task<SpecieViewModel> CreateSpecieViewModel(string artId);

        Task<bool> DeleteHunt(string id, string imagePath);
        Task<bool> DeleteLog(string id, string imagePath);
        Task<bool> DeleteHunter(string id, string imagePath);
        Task<bool> DeleteDog(string id, string imagePath);

    }

    public class HuntFactory : IHuntFactory
    {
        private readonly IBaseService<Jeger> _hunterService;
        private readonly INavigator _navigator;
        private readonly IBaseService<Jakt> _huntService;
        private readonly IBaseService<Logg> _logService;
        private readonly IBaseService<Dog> _dogService;
        private readonly IBaseService<Art> _specieService;
        private readonly IDialogService _dialogService;
        private readonly IFileManager _fileManager;
        private readonly Func<SpecieViewModel> _specieViewModelFactory;
        private readonly ISelectService<Art> _selectService;

        public HuntFactory(IBaseService<Jeger> hunterService, 
            INavigator navigator, 
            IBaseService<Jakt> huntService, 
            IBaseService<Logg> logService,
            IBaseService<Dog> dogService,
            IBaseService<Art> specieService,
            IDialogService dialogService,
            IFileManager fileManager,
            Func<SpecieViewModel> specieViewModelFactory,
            ISelectService<Art> selectService)
        {
            _hunterService = hunterService;
            _navigator = navigator;
            _huntService = huntService;
            _logService = logService;
            _dogService = dogService;
            _specieService = specieService;
            _dialogService = dialogService;
            _fileManager = fileManager;
            _specieViewModelFactory = specieViewModelFactory;
            _selectService = selectService;
        }

        public HuntFactory()
        {
        }

        public async Task<List<PickerItem>> CreateHunterPickerItems(List<string> selectedIds, string huntId = null)
        {
            var hunters = await _hunterService.GetItems();
            var hunterPickers = hunters.Select(h => new PickerItem
            {
                ID = h.ID,
                Title = $"{h.Fornavn}",
                ImageSource = Utility.GetImageSource(h.ImagePath),
                Selected = selectedIds.Exists(x => x == h.ID)
            }).ToList();

            hunterPickers = hunterPickers.OrderByDescending(o => o.Selected).ToList();
            if(huntId != null) 
            {
                var hunt = await _huntService.Get(huntId);
                hunterPickers = hunterPickers.OrderByDescending(
                        o => hunt.JegerIds.Any(h => h == o.ID)).ToList();
            }
            return hunterPickers;
        }

        public async Task<List<PickerItem>> CreateDogPickerItems(List<string> selectedIds, string huntId = null)
        {
            var dogs = await _dogService.GetItems();
            var dogPickers = dogs.Select(h => new PickerItem
            {
                ID = h.ID,
                Title = $"{h.Navn}",
                ImageSource = Utility.GetImageSource(h.ImagePath),
                Selected = selectedIds.Exists(x => x == h.ID)
            }).ToList();

            dogPickers = dogPickers.OrderByDescending(o => o.Selected).ToList();
            if (huntId != null)
            {
                var hunt = await _huntService.Get(huntId);
                dogPickers = dogPickers.OrderByDescending(
                        o => hunt.DogIds.Any(h => h == o.ID)).ToList();
            }
            return dogPickers;
        }

        public async Task<List<PickerItem>> CreateSpeciePickerItems(string selectedId)
        {
            var species = await _specieService.GetItems();
            var ids = await _selectService.GetItems();
            var speciePickers = species.Where(s => ids.Contains(s.ID)).Select(h => new PickerItem
            {
                ID = h.ID,
                Title = $"{h.Navn}",
                ImageSource = Utility.GetImageSource("bird.png"),
                Selected = selectedId == h.ID
            }).ToList();

            speciePickers = speciePickers.OrderByDescending(o => o.Selected).ToList();
            return speciePickers;
        }


        public async Task<SpecieViewModel> CreateSpecieViewModel(string artId)
        {
            var art = await _specieService.Get(artId);
            var itemVM = _specieViewModelFactory();
            itemVM.SetState(art, true);
            await itemVM.AfterNavigate();
            return itemVM;
        }

        public async Task<bool> DeleteHunt(string id, string imagePath)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jakta og alle loggføringer blir permanent slettet. Er du sikker?");
            if (ok)
            {
                var logs = await _logService.GetItems();
                foreach(var log in logs.Where(l => l.JaktId == id).ToList())
                {
                    await _logService.Delete(log.ID);
                    _fileManager.Delete(log.ImagePath);
                }

                await _huntService.Delete(id);
                _fileManager.Delete(imagePath);
            }
            return ok;
        }
        public async Task<bool> DeleteLog(string id, string imagePath)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Loggføringen blir permanent slettet. Er du sikker?");
            if (ok)
            {
                await _logService.Delete(id);
                _fileManager.Delete(imagePath);
            }
            return ok;
        }

        public async Task<bool> DeleteHunter(string id, string imagePath)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Jeger blir permanent slettet. Er du sikker?");
            if (ok)
            {
                var logs = await _logService.GetItems();
                foreach (var log in logs.Where(l => l.JegerId == id))
                {
                    log.JegerId = string.Empty;
                    await _logService.Save(log);
                }

                var hunts = await _huntService.GetItems();
                foreach (var hunt in hunts.Where(h => h.JegerIds.Any(hh => hh == id)).ToList())
                {
                    hunt.JegerIds.Remove(id);
                    await _huntService.Save(hunt);
                }

                await _hunterService.Delete(id);
                _fileManager.Delete(imagePath);
            }
            return ok;
        }

        public async Task<bool> DeleteDog(string id, string imagePath)
        {
            var ok = await _dialogService.ShowConfirmDialog("Bekreft sletting", "Hund blir permanent slettet. Er du sikker?");
            if (ok)
            {
                var logs = await _logService.GetItems();
                foreach (var log in logs.Where(l => l.DogId == id))
                {
                    log.DogId = string.Empty;
                    await _logService.Save(log);
                }

                var hunts = await _huntService.GetItems();
                foreach (var hunt in hunts.Where(h => h.DogIds.Any(hh => hh == id)).ToList())
                {
                    hunt.DogIds.Remove(id);
                    await _huntService.Save(hunt);
                }

                await _dogService.Delete(id);
                _fileManager.Delete(imagePath);
            }
            return ok;
        }

    }
}
