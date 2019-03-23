using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Services;

namespace HuntLog.Factories
{
    public interface IHunterFactory
    {
        Task<List<InputPickerItemViewModel>> CreateHunterPickerItems(List<string> selectedHunterIds);
    }

    public class HunterFactory : IHunterFactory
    {
        private readonly IHunterService _hunterService;
        private readonly INavigator _navigator;

        public HunterFactory(IHunterService hunterService, INavigator navigator)
        {
            _hunterService = hunterService;
            _navigator = navigator;
        }

        public async Task<List<InputPickerItemViewModel>> CreateHunterPickerItems(List<string> selectedHunterIds)
        {
            var hunters = await _hunterService.GetItems();
            var hunterPickers = hunters.Select(h => new InputPickerItemViewModel
            {
                ID = h.ID,
                Title = $"{h.Firstname} {h.Lastname}",
                ImageSource = Utility.GetImageSource(h.ImagePath),
                Selected = selectedHunterIds.Exists(x => x == h.ID)
            }).ToList();
            return hunterPickers;
        }
    }
}
