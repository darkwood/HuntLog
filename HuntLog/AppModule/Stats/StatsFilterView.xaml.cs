using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HuntLog.AppModule.Stats
{
    public partial class StatsFilterView : ContentView
    {
        private readonly StatsFilterViewModel _viewModel;

        public StatsFilterView()
        {
            InitializeComponent();
        }
    }

    public class StatsFilterViewModel : ViewModelBase
    {
        public Command DateRangeCommand { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public StatsFilterViewModel()
        {
            DateRangeCommand = new Command(async (obj) => await SetDate(obj));

            DateFrom = DateTime.Now.AddMonths(-1);
            DateTo = DateTime.Now;

        }

        private async Task SetDate(object obj)
        {
            switch (obj as string)
            {
                case "Forever":
                    DateFrom = DateTime.MinValue;
                    DateTo = DateTime.Now;
                    break;
                case "Year":
                    DateFrom = DateTime.Now.AddYears(-1);
                    DateTo = DateTime.Now;
                    break;
                case "Month":
                    DateFrom = DateTime.Now.AddMonths(-1);
                    DateTo = DateTime.Now;
                    break;
            }
            await Task.CompletedTask;

        }
    }
}
