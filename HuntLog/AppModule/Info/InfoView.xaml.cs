using System;
using System.Collections.Generic;
using HuntLog.Helpers;
using Xamarin.Forms;

namespace HuntLog.AppModule.Info
{
    public partial class InfoView : ContentPage
    {
        private readonly InfoViewModel _viewModel;

        public InfoView(InfoViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            BindingContext = _viewModel;
        }
    }

    public class InfoViewModel : ViewModelBase
    {
        public InfoViewModel()
        {
            ImageSource = Utility.GetImageFromAssets("info.jpg");

        }
    }
}
