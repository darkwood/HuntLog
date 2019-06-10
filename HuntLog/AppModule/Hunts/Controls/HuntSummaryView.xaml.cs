using System;
using System.Collections.Generic;
using HuntLog.Helpers;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts.Controls
{
    public partial class HuntSummaryView : ContentView
    {
        public ImageSource ImageSource => Utility.GetImageSource("");
        public string Name => "Tore på spore";
        public HuntSummaryView()
        {
            if (DesignMode.IsDesignModeEnabled)
            {
                BindingContext = this;
            }
            InitializeComponent();
        }
    }
}
