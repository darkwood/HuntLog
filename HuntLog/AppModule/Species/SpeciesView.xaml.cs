using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HuntLog.Factories;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Species
{
    public partial class SpeciesView : ContentPage
    {
        private readonly SpeciesViewModel _viewModel;

        public SpeciesView(SpeciesViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.InitializeAsync();
            base.OnAppearing();
        }
    }
    public class SpeciesGroup : ObservableCollection<SpecieViewModel>
    {
        public String Name { get; private set; }
        public String ShortName { get; private set; }

        public SpeciesGroup(String Name, String ShortName)
        {
            this.Name = Name;
            this.ShortName = ShortName;
        }
    }

    public class SpeciesViewModel : ViewModelBase
    {
        private readonly IBaseService<Art> _specieService;
        private readonly IBaseService<ArtGroup> _specieGroupService;
        private readonly ISelectService<Art> _selectService;
        private readonly INavigator _navigator;
        private readonly Func<SpecieViewModel> _specieViewModelFactory;
        private readonly IHuntFactory _huntFactory;

        public ObservableCollection<SpeciesGroup> SpeciesGroup { get; set; }

        public Command AddCommand { get; set; }
        public Command DeleteItemCommand { get; set; }

        public SpeciesViewModel(IBaseService<Art> specieService, 
            IBaseService<ArtGroup> specieGroupService, 
            ISelectService<Art> selectService,
            INavigator navigator, 
            Func<SpecieViewModel> specieViewModelFactory, 
            IHuntFactory huntFactory)
        {
            _specieService = specieService;
            _specieGroupService = specieGroupService;
            _selectService = selectService;
            _navigator = navigator;
            _specieViewModelFactory = specieViewModelFactory;
            _huntFactory = huntFactory;

            AddCommand = new Command(async () => await AddItem());
            //DeleteItemCommand = new Command(async (item) => await DeleteItem(item));

        }

        private async Task AddItem()
        {
            await _navigator.PushAsync<SpecieViewModel>(
                    beforeNavigate: (vm) => vm.SetState(null, true));
        }

        //private async Task DeleteItem(object item)
        //{
        //    var ok = await _huntFactory.DeleteSpecie((item as SpecieViewModel).ID, (item as SpecieViewModel).ImagePath);
        //    if (ok)
        //    {
        //        await FetchData();
        //    }
        //}
        public async Task InitializeAsync()
        {
            await FetchData();
        }

        public async Task FetchData()
        {
            IsBusy = true;

            SpeciesGroup = new ObservableCollection<SpeciesGroup>();
            var items = await _specieService.GetItems();
            var groups = await _specieGroupService.GetItems();
            var selectedArts = await _selectService.GetItems();
            var itemVMs = items.Select(i =>
            {
                var itemVM = _specieViewModelFactory();
                itemVM.SetState(i, selectedArts.Any(x => x == i.ID));
                return itemVM;
            });

            //var itemsInGroups = itemVMs.GroupBy(g => g.GroupId);
            
            foreach (var g in groups.Where(gr => gr.ID != "100"))
            {
                var group = new SpeciesGroup(g.Navn, "");
                foreach (var specie in itemVMs.Where(x => x.GroupId.ToString() == g.ID))
                {
                    group.Add(specie);
                }
                SpeciesGroup.Add(group);
            }

            IsBusy = false;
        }
    }
}