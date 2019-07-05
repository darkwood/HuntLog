using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Factories;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.CustomFields
{
    public partial class CustomFieldsView : ContentPage
    {
        private readonly CustomFieldsViewModel _viewModel;

        public CustomFieldsView(CustomFieldsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.Initialize();
            base.OnAppearing();
        }
    }
    public class CustomFieldsGroup : ObservableCollection<CustomFieldViewModel>
    {
        public String Name { get; private set; }
        public String ShortName { get; private set; }

        public CustomFieldsGroup(String Name, String ShortName)
        {
            this.Name = Name;
            this.ShortName = ShortName;
        }
    }

    public class CustomFieldsViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly ICustomFieldFactory _factory;

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }
        public ObservableCollection<CustomFieldsGroup> CustomFieldsGroup { get; set; }

        public CustomFieldsViewModel(ICustomFieldFactory factory,
            INavigator navigator)
        {
            _navigator = navigator;;
            _factory = factory;

            AddCommand = new Command(async () => await AddItem());
        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<CustomFieldViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null, true));
        }

        public async Task Initialize()
        {
            await FetchData();
        }

        public async Task FetchData()
        {
            IsBusy = true;

            CustomFieldsGroup = new ObservableCollection<CustomFieldsGroup>();
            var groups = await _factory.CreateCustomFieldsGroups(null, false);
            foreach(var group in groups)
            {
                CustomFieldsGroup.Add(group);
            }
            IsBusy = false;
        }
    }
}