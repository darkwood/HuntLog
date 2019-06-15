using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.AppModule.CustomFields;
using HuntLog.AppModule.Species;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Models;
using HuntLog.Services;

namespace HuntLog.Factories
{
    public interface ICustomFieldFactory
    {
        Task<List<CustomFieldsGroup>> CreateCustomFieldsGroups(Logg log, bool selectedOnly);
        Task<List<CustomFieldViewModel>> CreateCustomFields(Logg log);
    }

    public class CustomFieldFactory : ICustomFieldFactory
    {
        private readonly Func<CustomFieldViewModel> _customFieldViewModelFactory;
        private readonly ISelectService<LoggType> _selectService;
        private readonly IBaseService<LoggType> _customFieldService;
        private readonly IBaseService<LoggTypeGroup> _customFieldGroupService;

        public CustomFieldFactory(
            Func<CustomFieldViewModel> customFieldViewModelFactory,
            ISelectService<LoggType> selectService,
            IBaseService<LoggType> customFieldService,
            IBaseService<LoggTypeGroup> customFieldGroupService)
        {

            _customFieldViewModelFactory = customFieldViewModelFactory;
            _selectService = selectService;
            _customFieldService = customFieldService;
            _customFieldGroupService = customFieldGroupService;

        }

        public async Task<List<CustomFieldViewModel>> CreateCustomFields(Logg log)
        {
            var items = await _customFieldService.GetItems();
            var selectedLoggTypes = await _selectService.GetItems();
            var itemVMs = items.Select(i =>
            {
                var itemVM = _customFieldViewModelFactory();
                itemVM.SetState(i, selectedLoggTypes.Any(x => x == i.Key));
                if (log != null)
                {
                    itemVM.Value = GetValueFromLog(i.Key, log);
                }

                return itemVM;
            });
            return itemVMs.ToList();
        }

        public async Task<List<CustomFieldsGroup>> CreateCustomFieldsGroups(Logg log, bool selectedOnly)
        {
            var customFieldsGroup = new List<CustomFieldsGroup>();
            var groups = await _customFieldGroupService.GetItems();
            var itemVMs = await CreateCustomFields(log);

            //var itemsInGroups = itemVMs.GroupBy(g => g.GroupId);

            foreach (var g in groups)
            {
                var group = new CustomFieldsGroup(g.Navn, "");
                foreach (var customField in itemVMs.Where(x => x.GroupId.ToString() == g.ID))
                {
                    group.Add(customField);
                }
                customFieldsGroup.Add(group);
            }

            return customFieldsGroup;
        }

        private string GetValueFromLog(string key, Logg _log)
        {
            switch (key)
            {
                case "Gender":
                    return _log.Gender;
                case "Weather":
                    return _log.Weather;
                case "Age":
                    return _log.Age;
                case "WeaponType":
                    return _log.WeaponType;
                case "Weight":
                    return _log.Weight.ToString();
                case "ButchWeight":
                    return _log.ButchWeight.ToString();
                case "Tags":
                    return _log.Tags.ToString();
                default:
                    throw new KeyNotFoundException(key);
            }
        }
    }
}
