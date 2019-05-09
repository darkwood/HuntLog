using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Xamarin.Forms;
using HuntLog.Extensions;
using HuntLog.Services;
using HuntLog.InputViews;

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

        public Command DateRangeCommand { get; set; }
        public Command FilterDateCommand { get; set; }
        public Command DateFromCommand { get; set; }
        public Command DateToCommand { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string DateFromTo => GetFormattedDateRange();
        public bool Visible { get; set; }
        public Command FocusDateFromCommand { get; set; }
        public Command FocusDateToCommand { get; set; }


        public static Action DateChangedAction { get; set; }
        public StatsFilterViewModel(INavigator navigator)
        {

            DateRangeCommand = new Command(async (obj) => await SetDate(obj));
            FilterDateCommand = new Command(() => 
            { 
                Visible = !Visible;
                if (!Visible)
                {
                    DateChangedAction?.Invoke();
                }
            });

            DateFrom = DateTime.Now.AddMonths(-1);
            DateTo = DateTime.Now;

            _navigator = navigator;
        }


        private async Task SetDate(object obj)
        {
            switch (obj as string)
            {
                case "Alle":
                    DateFrom = DateTime.Parse("1990-01-01");
                    DateTo = DateTime.Now;
                    break;
                case "Siste år":
                    DateFrom = DateTime.Now.AddYears(-1);
                    DateTo = DateTime.Now;
                    break;
                case "Siste måned":
                    DateFrom = DateTime.Now.AddMonths(-1);
                    DateTo = DateTime.Now;
                    break;
            }
            //DateFromTo = GetFormattedDateRange(obj as string);
            await Task.CompletedTask;

        }

        private string GetFormattedDateRange(string rangeString = null)
        {
            return rangeString == null ? $"{DateFrom.ToNoString()} - {DateTo.ToNoString()}" : rangeString;
        }
    }
}
