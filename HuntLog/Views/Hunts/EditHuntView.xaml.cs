using System;
using System.Collections.Generic;
using HuntLog.ViewModels.Hunts;
using Xamarin.Forms;

namespace HuntLog.Views.Hunts
{
    public partial class EditHuntView : ContentPage
    {
        private readonly EditHuntViewModel _viewModel;

        public EditHuntView(EditHuntViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            await _viewModel.OnAppearing();
            base.OnAppearing();
        }
    }
}
