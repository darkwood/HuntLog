using System;
using System.Collections.Generic;
using HuntLog.ViewModels.Hunters;
using Xamarin.Forms;

namespace HuntLog.Views.Hunters
{
    public partial class HunterView : ContentPage
    {
        private readonly HunterViewModel _viewModel;

        public HunterView(HunterViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = viewModel;
            _viewModel = viewModel;
        }
    }
}
