using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Bogus;
using HuntLog.Models;
using HuntLog.Services;
using Xamarin.Forms;

namespace HuntLog.AppModule.Stats
{
    public partial class StatsView : ContentPage
    {
        private readonly StatsViewModel _viewModel;

        public StatsView(StatsViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }

    public class StatsViewModel : ViewModelBase
    {
        private readonly IBaseService<Jakt> _huntService;
        private readonly IBaseService<Logg> _logService;
        private readonly IBaseService<Jeger> _hunterService;
        private readonly IBaseService<Dog> _dogService;
        private readonly IBaseService<Art> _specieService;
        private readonly INavigator _navigator;

        public Command CreateDummyDataCommand { get; set; }
        public Command DeleteCommand { get; set; }
        public IEnumerable<Jeger> Hunters { get; private set; }
        public IEnumerable<Jakt> Hunts { get; private set; }
        public IEnumerable<Logg> Logs { get; private set; }
        public IEnumerable<Art> Species { get; private set; }
        public IEnumerable<Dog> Dogs { get; private set; }

        public Command MapCommand { get; set; }



        public string Info { get; set; }

        public StatsViewModel() { }
        public StatsViewModel(IBaseService<Jakt> huntService,
                              IBaseService<Logg> logService,
                              IBaseService<Jeger> hunterService,
                              IBaseService<Dog> dogService,
                              IBaseService<Art> specieService,
                              INavigator navigator )
        {
            CreateDummyDataCommand = new Command(async () =>  await GenerateDummyData());
            DeleteCommand = new Command(async () => await DeleteAll());
            _huntService = huntService;
            _logService = logService;
            _hunterService = hunterService;
            _dogService = dogService;
            _specieService = specieService;
            _navigator = navigator;

            MapCommand = new Command(async () => { await ShowMap(); });

        }

        private async Task ShowMap()
        {
            await _navigator.PushAsync<StatsMapViewModel>();
        }


        private async Task DeleteAll()
        {
            Info = "Deleting...";
            var timer = new Timer();
            timer.Start();

            await _huntService.DeleteAll();
            await _logService.DeleteAll();
            await _hunterService.DeleteAll();
            await _dogService.DeleteAll();

            timer.Stop();
            Info = "All data deleted. It took " + timer.Interval + " ms";
            timer.Dispose();
        }

        private async Task GenerateDummyData()
        {
            Info = "Generating data... ";


            Species = await _specieService.GetItems();

            for (var j = 1; j <= 10; j++)
            {
                var jeger = CreateJeger(j);
                await _hunterService.Save(jeger);
            }
            Hunters = await _hunterService.GetItems();

            /****************************************/

            for (var d = 1; d <= 10; d++)
            {
                var dog = CreateDog(d);
                await _dogService.Save(dog);
            }
            Dogs = await _dogService.GetItems();

            /****************************************/

            for (var i = 1; i <= 10; i++)
            {
                var item = CreateHunt(i);
                await _huntService.Save(item);

                for (var x = 1; x <= 10; x++)
                {
                    var log = CreateLog(x*i, i);
                    await _logService.Save(log);
                }
            }

            Hunts = await _huntService.GetItems();
            Logs = await _logService.GetItems();

            Info += $"Done! {Logs.Count()} logs generated, in {Hunts.Count()} hunts.";

        }

        private Dog CreateDog(int id)
        {
            var faker = new Faker<Dog>("nb_NO")
                .RuleFor(o => o.ID, f => id.ToString())
                .RuleFor(u => u.Navn, (f, u) => f.Hacker.Noun())
                .RuleFor(u => u.Rase, (f, u) => f.Hacker.Verb())
                .RuleFor(u => u.Lisensnummer, (f, u) => f.Hacker.Phrase())
                ;
            var dog = faker.Generate();
            return dog;
        }

        private Jeger CreateJeger(int id) 
        {
            var faker = new Faker<Jeger>("nb_NO")
                .RuleFor(o => o.ID, f => id.ToString())
                .RuleFor(u => u.Fornavn, (f, u) => f.Name.FirstName())
                .RuleFor(u => u.Etternavn, (f, u) => f.Name.LastName())
                .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
                .RuleFor(u => u.Phone, (f, u) => f.Phone.PhoneNumber())
                ;
            var jeger = faker.Generate();

            return jeger;
        }

        private Jakt CreateHunt(int id)
        {
            var faker = new Faker<Jakt>("nb_NO")
                .RuleFor(o => o.ID, f => id.ToString())
                .RuleFor(u => u.Sted, (f, u) => f.Address.City())
                .RuleFor(u => u.Notes, (f, u) => f.Rant.Review())
                ;
            var hunt = faker.Generate();

            hunt.DatoFra = DateTime.Today.AddDays(rnd(-2000, 0));
            hunt.JegerIds = Hunters.Select(x => x.ID).Skip(rnd(0, 5)).Take(rnd(0, 5)).ToList();
            hunt.DogIds = Dogs.Select(x => x.ID).Skip(rnd(0, 5)).Take(rnd(0, 5)).ToList();
            hunt.DatoTil = hunt.DatoFra.AddDays(rnd(0, 10));

            return hunt;
        }

        private static int rnd(int from, int to)
        {
            return new Random(DateTime.Now.Millisecond).Next(from, to);
        }

        private Logg CreateLog(int id, int huntId)
        {
            var faker = new Faker<Logg>("nb_NO")
                .RuleFor(o => o.ID, f => id.ToString())
                .RuleFor(u => u.Notes, (f, u) => f.Rant.Review())
                ;
            var log = faker.Generate();


            log.JaktId = huntId.ToString();
            log.Dato = DateTime.Now.AddHours(rnd(-5, 5)).AddMinutes(rnd(0, 59));
            log.Latitude = "63." + rnd(3800, 4300);
            log.Longitude = "10." + rnd(2000, 3000);
            log.JegerId = Hunters.ElementAt(rnd(0, Hunters.Count() - 1)).ID;
            log.DogId = Dogs.ElementAt(rnd(0, Dogs.Count() - 1)).ID;
            log.ArtId = Species.ElementAt(rnd(0, Species.Count() - 1)).ID;
            log.Sett = rnd(0, 10);
            log.Skudd = rnd(0, 2);
            log.Treff = rnd(0, log.Skudd);

            return log;
        }
    }
}