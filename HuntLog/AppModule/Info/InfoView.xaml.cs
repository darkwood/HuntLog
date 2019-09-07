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
            webView.Source = "https://docs.google.com/forms/d/e/1FAIpQLSc3SL850itOSFd0nIqKnT8iovy6m3rKHrH_3_F3ck2mr9FP6g/viewform?usp=sf_link";
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
