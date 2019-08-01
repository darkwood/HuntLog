﻿using System;
using Xnapshot;

namespace HuntLog.Xnapshot
{
    public class HuntLogScreenshots : Screenshots
    {
        public HuntLogScreenshots() : base(
          "iOS-12-4",
          "/Users/tom/Apps/HuntLog/screenshots",
          "/Users/tom/Apps/HuntLog/HuntLog.iOS/bin/iPhoneSimulator/Release/HuntLog.iOS.app",
          new[] {
              //"iPhone-XS-Max",
              "iPhone-XS",
              //"iPhone-XR",
              //"iPhone-8-Plus",
              "iPhone-8",
              //"iPhone-SE"
          })
        {
        }

        protected override void SetAppStateForScreenshot1()
        {
        }

        protected override void SetAppStateForScreenshot2()
        {
            App.Tap(c => c.Marked("Oppsett"));
        }
    }
}