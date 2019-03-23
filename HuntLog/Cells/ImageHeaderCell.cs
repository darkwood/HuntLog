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

        private View GetButtons()
        {
            var btnLayout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                VerticalOptions = LayoutOptions.EndAndExpand,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                InputTransparent = true,
                CascadeInputTransparent = false,

            };
            btnLayout.Children.Add(CreateCircleImageButton("camera.png", HuntConfig.CapturePhoto));
            btnLayout.Children.Add(CreateCircleImageButton("photos.png", HuntConfig.OpenLibrary));
            return btnLayout;
        }

        private CircleImage CreateCircleImageButton(string imagepath, string commandArg)
        {
            var img = new CircleImage
            {
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.End,
                BorderThickness = 2,
                BorderColor = Color.Black,
                FillColor = Color.FromHex("#66FFFFFF"),
                Source = ImageSource.FromFile(imagepath),
                Aspect = Aspect.AspectFit,
                Opacity = 0.8,
                HeightRequest = 60,
                WidthRequest = 60,
                Margin = 5,
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
