using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;
using HuntLog.Helpers;
using Plugin.Media.Abstractions;
using HuntLog.Cells;
using HuntLog.Factories;
using HuntLog.InputViews;

namespace HuntLog.AppModule.CustomFields
{
    public partial class CustomFieldView : ContentPage
    {
        private readonly CustomFieldViewModel _viewModel;

        public CustomFieldView(CustomFieldViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }

    public class CustomFieldViewModel : ViewModelBase
    {
        private readonly IBaseService<LoggType> _customFieldService;
        private readonly INavigator _navigator;
        private readonly ISelectService<LoggType> _selectService;
        private readonly IHuntFactory _huntFactory;
        private LoggType _dto;

        public ObservableCollection<LoggType> CustomFields { get; set; }

        public Command SaveCommand { get; set; }
        public Command CancelCommand { get; set; }
        public Command ItemTappedCommand { get; set; }
        public Command EditItemCommand { get; set; }
        public Command DeleteCommand { get; set; }

        public CellAction ImageAction { get; set; }

        public string Name { get; set; }
        public int GroupId { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
        public string Value { get; set; }



        public CustomFieldViewModel(IBaseService<LoggType> customFieldService, INavigator navigator, ISelectService<LoggType> selectService, IHuntFactory huntFactory)
        {
            _customFieldService = customFieldService;
            _navigator = navigator;
            _selectService = selectService;
            _huntFactory = huntFactory;

            //SaveCommand = new Command(async () => await Save());
            //DeleteCommand = new Command(async () => await Delete());
            ItemTappedCommand = new Command(async () => await Tapped());
            CancelCommand = new Command(async () => { await PopAsync(); });
        }

        private async Task PopAsync()
        {
            await _navigator.PopAsync();
        }

        private async Task Save()
        {
            LoggType dto = BuildDto();
            await _customFieldService.Save(dto);
            await PopAsync();
        }

        private async Task Tapped()
        {
            if (Selected)
            {
                await _selectService.Delete(ID);
            }
            else
            {
                await _selectService.Add(ID);
            }

            Selected = !Selected;
            //await _navigator.PushAsync<CustomFieldViewModel>(beforeNavigate: (arg) => arg.SetState(_dto, Selected));
        }

        //private async Task Delete()
        //{
        //    var ok = await _huntFactory.DeleteCustomField(ID, ImagePath);
        //    if (ok)
        //    {
        //        await PopAsync();
        //    }
        //}

        public void SetState(LoggType dto, bool selected)
        {
            _dto = dto ?? new LoggType();
            ID = _dto.Key;
            Name = _dto.Navn;
            Description = _dto.Beskrivelse;
            GroupId = _dto.GroupId;
            Title = ID == null ? "Nytt felt" : _dto.Navn;
            Selected = selected;
        }

        public object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
        }

        protected LoggType BuildDto()
        {
            return new LoggType
            {
                ID = ID ?? Guid.NewGuid().ToString(),
                Created = DateTime.Now,
                Navn = Name,
                Beskrivelse = Description,
                GroupId = GroupId
            };
        }
    }
}