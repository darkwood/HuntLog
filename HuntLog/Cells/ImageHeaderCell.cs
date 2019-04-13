using System;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Services;
using ImageCircle.Forms.Plugin.Abstractions;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace HuntLog.Cells
{
    public class ImageHeaderCell : BaseCell
    {
        public static readonly BindableProperty SourceProperty =
            BindableProperty.Create(
                nameof(Source), 
                typeof(ImageSource), 
                typeof(ImageHeaderCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var src = newValue as ImageSource;
                    ((ImageHeaderCell)bindable).CellImage.Source = src;
                    //((ImageHeaderCell)bindable).CellImage.IsVisible = newValue != null;
                    ((ImageHeaderCell)bindable).Buttons.IsVisible = true;
                }
            );
        
        public string Source
        {
            get { return (string)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create(
            nameof(HeightRequest), 
            typeof(string), 
            typeof(ImageHeaderCell), 
            "150",
            propertyChanged: (bindable, oldValue, newValue) => {
                var height = double.Parse(newValue as string);
                ((ImageHeaderCell)bindable).View.HeightRequest = height;
            });



        public string HeightRequest
        {
            get { return (string)GetValue(HeightRequestProperty); }
            set { SetValue(HeightRequestProperty, value); }
        }

        public Image CellImage { get; private set; }
        public View Buttons { get; private set; }

        public ImageHeaderCell()
        {
            var viewLayout = new Grid { HeightRequest = double.Parse(HeightRequest) };
            viewLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            viewLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            CreateImage();

            Buttons = GetButtons();

            viewLayout.Children.Add(CellImage, 0, 0);
            viewLayout.Children.Add(Buttons, 1, 0);

            View = viewLayout;
        }

        private void CreateImage()
        {
            CellImage = new Image
            {
                Aspect = Aspect.AspectFill
            };
            CellImage.GestureRecognizers.Add(CreateTapGestureRecognizer());
        }

        private View GetButtons()
        {
            var btnLayout = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                InputTransparent = true,
                CascadeInputTransparent = false,

            };
            btnLayout.Children.Add(CreateButton("camera.png", HuntConfig.CapturePhoto), 0, 0);
            btnLayout.Children.Add(CreateButton("photos.png", HuntConfig.OpenLibrary), 0 , 1);
            return btnLayout;
        }

        private ImageButton CreateButton(string imagepath, string commandArg)
        {
            var img = new ImageButton
            {
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand,
                BackgroundColor = Color.FromHex("EEEEEE"),
                Source = ImageSource.FromFile(imagepath),
            };
            img.Command = new Command(async () => await EditImage(commandArg));
            //img.GestureRecognizers.Add(CreateTapGestureRecognizer(commandArg));
            return img;
        }

        private TapGestureRecognizer CreateTapGestureRecognizer(string commandArg = null)
        {
            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += async (s, e) =>
            {
                await EditImage(commandArg);
            };
            return gestureRecognizer;
        }

        private async Task EditImage(object shortcut)
        {
            await _navigator.PushAsync<InputImageViewModel>(
                beforeNavigate: async (arg) =>
                {
                    await arg.InitializeAsync(CellImage.Source, CellAction.Save, CellAction.Delete);
                },
                afterNavigate: async (arg) => await arg.OnAfterNavigate(shortcut as string),
                shortcut == null);
        }
    }
}
