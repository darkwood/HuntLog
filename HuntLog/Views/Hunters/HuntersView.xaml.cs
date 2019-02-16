using System;
using System.Collections.Generic;
using HuntLog.ViewModels.Hunters;
using Xamarin.Forms;

namespace HuntLog.Views.Hunters
{
    public partial class HuntersView : ContentPage
    {
        private readonly HuntersViewModel _viewModel;

        public HuntersView(HuntersViewModel viewModel)
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
}
