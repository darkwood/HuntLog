using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using HuntLog.Interfaces;
using Xamarin.Forms;

namespace HuntLog
{
    public static class Utility
    {
        private static readonly string PLACEHOLDER_PHOTO = "placeholder.jpg";
        public static Color PRIMARY_COLOR => Color.FromHex("#597a59");

        public static ImageSource GetImageSource(string imageFilename)
        {
            if (String.IsNullOrEmpty(imageFilename))
            {
                return ImageSource.FromResource("HuntLog.Assets."+PLACEHOLDER_PHOTO);
            }
            else
            {
                var filepath = DependencyService.Get<IFileUtility>().GetFilePath(imageFilename);
                return ImageSource.FromFile(filepath);
            }
        }

        public static string GetImageFilename(string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && filePath.Contains("/"))
            {
                filePath = filePath.Substring(filePath.LastIndexOf("/", StringComparison.CurrentCulture) + 1);
            }
            return filePath;
        }
        public static void LogError(Exception ex)
        {
            Debug.WriteLine(ex.Message, ex.InnerException, ex.StackTrace);
        }

        public static async Task AnimateButton(VisualElement btn)
        {
            await btn.ScaleTo(0.75, 50, Easing.Linear);
            await btn.ScaleTo(1, 50, Easing.Linear);
        }
    }
}
