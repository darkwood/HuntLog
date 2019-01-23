using System;
using System.Collections.Generic;
using HuntLog.ViewModels.Hunts;
using Xamarin.Forms;

namespace HuntLog.Views.Hunts
{
    public partial class HuntsView : ContentPage
    {
        private readonly HuntsViewModel _viewModel;

        public HuntsView(HuntsViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
