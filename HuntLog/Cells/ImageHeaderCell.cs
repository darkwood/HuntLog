using System;
using System.Windows.Input;
using HuntLog.Helpers;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace HuntLog.Cells
{
    public class ImageHeaderCell : ViewCell
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(Command), typeof(ImageHeaderCell), null);

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public static readonly BindableProperty ImagePathProperty =
            BindableProperty.Create(
                nameof(ImagePath), 
                typeof(string), 
                typeof(ImageHeaderCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var path = newValue as string;
                    ((ImageHeaderCell)bindable).CellImage.Source = path == null ? null : Utility.GetImageSource(path);
                    ((ImageHeaderCell)bindable).CellImage.IsVisible = newValue != null;
                    ((ImageHeaderCell)bindable).Buttons.IsVisible = newValue == null;
                }
            );
        
        public string ImagePath
        {
            get { return (string)GetValue(ImagePathProperty); }
            set { SetValue(ImagePathProperty, value); }
        }

        public static readonly BindableProperty HeightRequestProperty = BindableProperty.Create(
            nameof(HeightRequest), 
            typeof(string), 
            typeof(ImageHeaderCell), 
            "150",
            propertyChanged: (bindable, oldValue, newValue) => {
                ((ImageHeaderCell)bindable).CellImage.HeightRequest = double.Parse(newValue as string);
            });

        public string HeightRequest
        {
            get { return (string)GetValue(HeightRequestProperty); }
            set { SetValue(HeightRequestProperty, value); }
        }

        public Image CellImage { get; private set; }
        public Grid Buttons { get; private set; }

        public ImageHeaderCell()
        {
            var viewLayout = new Grid();
            CreateImage();

            Buttons = GetButtons();

            viewLayout.Children.Add(CellImage);
            viewLayout.Children.Add(Buttons);

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

        private Grid GetButtons()
        {
            var g = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.LightGray
            };
            g.Children.Add(CreateCircleImageButton("camera.png", "takephoto"), 0, 0);
            g.Children.Add(CreateCircleImageButton("photos.png", "openlibrary"), 1, 0);
            return g;
        }

        private CircleImage CreateCircleImageButton(string imagepath, string commandArg)
        {
            var img = new CircleImage
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                BorderThickness = 2,
                BorderColor = Color.Black,
                FillColor = Color.FromHex("#66FFFFFF"),
                Source = ImageSource.FromFile(imagepath),
                Aspect = Aspect.AspectFit,
                Opacity = 0.8,
                HeightRequest = 80,
                WidthRequest = 80,
                Margin = 40,
            };
            img.GestureRecognizers.Add(CreateTapGestureRecognizer(commandArg));
            return img;
        }

        private TapGestureRecognizer CreateTapGestureRecognizer(string commandArg = null)
        {
            var gestureRecognizer = new TapGestureRecognizer();

            gestureRecognizer.Tapped += (s, e) =>
            {
                if (Command != null && Command.CanExecute(null))
                {
                    Command.Execute(commandArg);
                }
            };
            return gestureRecognizer;
        }
    }
}
