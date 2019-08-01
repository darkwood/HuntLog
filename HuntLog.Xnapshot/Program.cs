using System;

namespace HuntLog.Xnapshot
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var screenshots = new HuntLogScreenshots();
            screenshots.TakeScreenshots();

            Environment.Exit(0);
        }
    }
}
