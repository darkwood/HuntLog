using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using HuntLog.Extensions;
using HuntLog.Services;
using HuntLog.InputViews;
using HuntLog.Models;
using System.Linq;
using HuntLog.Factories;
using HuntLog.Helpers;

namespace HuntLog.AppModule.Stats
{
    public partial class StatsFilterView : ContentView
    {

        public StatsFilterView()
        {
            InitializeComponent();
        }


        //void Handle_Clicked(object sender, System.EventArgs e)
        //{
        //    var newHeight = filter.Height > 50 ? 50 : 300;
        //    filter.LayoutTo(new Rectangle(filter.Bounds.X, filter.Bounds.Y, filter.Bounds.Width, newHeight), 500, Easing.CubicOut);
        //}
    }

    public class StatsFilterViewModel : ViewModelBase
    {
        private readonly INavigator _navigator;
        private readonly IHuntFactory _huntFactory;

        public Command DateRangeCommand { get; set; }
        public Command FilterCommand { get; set; }
        public Command DateFromCommand { get; set; }
        public Command DateToCommand { get; set; }
        public Command FilterHunterCommand { get; set; }
        public Command FocusDateFromCommand { get; set; }
        public Command FocusDateToCommand { get; set; }

        public PickerItem SelectedHunter { get; set; }
        public string HunterName => SelectedHunter?.Title ?? "Alle jegere";
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string FilterSummary => GetFilterSummary();
        public bool ShowFilterView { get; set; }
        public bool Visible { get; set; }
        public string UpDownIcon => Visible ? FontAwesomeIcons.AngleUp : FontAwesomeIcons.AngleDown;

        public static Action FilterChangedAction { get; set; }
        public DateTime Today => DateTime.Now.Date.AddDays(1).AddSeconds(-1);

        public StatsFilterViewModel(INavigator navigator, IHuntFactory huntFactory)
        {
            _huntFactory = huntFactory;
            _navigator = navigator;

            DateRangeCommand = new Command((obj) =>
            {
                SetDate(obj);
                FilterChangedAction?.Invoke();
            });

            FilterCommand = new Command(() => 
            { 
                Visible = !Visible;
            });

            FilterHunterCommand = new Command(async () => {

                var selected = new List<string>();
                if(SelectedHunter != null)
                {
                    selected.Add(SelectedHunter.ID);
                }
                var hunterItems = await _huntFactory.CreateHunterPickerItems(selected);

                await _navigator.PushAsync<InputPickerViewModel>(
                    afterNavigate: async (arg) => await arg.InitializeAsync(
                        value: hunterItems,
                        completeAction: (obj) => {
                            SelectedHunter = obj?.FirstOrDefault(x => x.Selected);
                            FilterChangedAction?.Invoke();
                        }
                        )
                    );
            });

            SetDate("Alle");
        }

        public void SetVisibility(bool show)
        {
            ShowFilterView = show;
        }


        private void SetDate(object obj)
        {
            switch (obj as string)
            {
                case "Alle":
                    DateFrom = DateTime.Parse("1990-01-01");
                    DateTo = DateTime.Parse("2999-01-01");
                    break;
                case "Siste år":
                    DateFrom = DateTime.Now.AddYears(-1);
                    DateTo = Today;
                    break;
                case "Siste måned":
                    DateFrom = DateTime.Now.AddMonths(-1);
                    DateTo = Today;
                    break;
            }
        }

        private string GetFilterSummary(string rangeString = null)
        {
            var range = rangeString == null ? $"{DateFrom.ToShortDateString()} - {DateTo.ToShortDateString()}" : rangeString;
            return $"{HunterName}, {range}";
        }
    }
}
