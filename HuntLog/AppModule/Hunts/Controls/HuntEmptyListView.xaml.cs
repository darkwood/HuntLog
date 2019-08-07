using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace HuntLog.AppModule.Hunts.Controls
{
    public partial class HuntEmptyListView : ContentView
    {
        public HuntEmptyListView()
        {
            InitializeComponent();
        }

        public async Task FadeOutOverlay()
        {
            await bg.FadeTo(0, 1000, Easing.CubicIn);
        }
    }
}
