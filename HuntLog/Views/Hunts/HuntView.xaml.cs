using System;
using HuntLog.ViewModels.Hunts;
using Xamarin.Forms;

namespace HuntLog.Views.Hunts
{
    public partial class HuntView : ContentPage
    {
        public HuntView()
        {
            InitializeComponent();
            BindingContext = new HuntViewModel(null)
            {
                Location = "Høylandet",
                Title = "Tittel for siden",
                DateFrom = DateTime.Now.AddDays(-10),
                DateTo = DateTime.Now.AddDays(-4)
            };
        }
            
        public HuntView(HuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}
