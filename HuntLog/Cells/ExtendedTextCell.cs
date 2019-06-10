using System;
using System.Windows.Input;
using HuntLog.Helpers;
using HuntLog.InputViews;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;

namespace HuntLog.Cells
{
    public class ExtendedTextCell : BaseCell
    {
        public static readonly BindableProperty CommandProperty = 
            BindableProperty.Create(
                nameof(Command), 
                typeof(Command), 
                typeof(ExtendedTextCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    //((ExtendedTextCell)bindable).ViewLayout.GestureRecognizers.Add(((ExtendedTextCell)bindable).GestureRecognizer);
                }
            );

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text), 
                typeof(string), 
                typeof(ExtendedTextCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                ((ExtendedTextCell)bindable).TextLabel.Text = newValue as string;
                }
            );
        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Label TextLabel { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty DetailProperty = BindableProperty.Create(
            nameof(Detail), 
            typeof(string), 
            typeof(ExtendedTextCell), 
            "",
            propertyChanged: (bindable, oldValue, newValue) => 
            {
                ((ExtendedTextCell)bindable).DetailLabel.IsVisible = true;
                ((ExtendedTextCell)bindable).DetailLabel.Text = newValue as string;
            });

        public string Detail
        {
            get { return (string)GetValue(DetailProperty); }
            set { 
            SetValue(DetailProperty, value); }
        }

        public Label DetailLabel { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty Text2Property = BindableProperty.Create(
            nameof(Text2),
            typeof(string),
            typeof(ExtendedTextCell),
            "",
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                var text = (newValue as string);
                if (text != null && text.Length > 30)
                {
                    text = text.Substring(0, 26) + "...";
                }
            ((ExtendedTextCell)bindable).Text2Label.IsVisible = true;
            ((ExtendedTextCell)bindable).Text2Label.Text = text;
            });

        public string Text2
        {
            get { return (string)GetValue(Text2Property); }
            set
            {
                SetValue(Text2Property, value);
            }
        }

        public Label Text2Label { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty SpinnerActiveProperty = BindableProperty.Create(
            nameof(SpinnerActive),
            typeof(bool),
            typeof(ExtendedTextCell),
            false,
            propertyChanged: (bindable, oldValue, newValue) => {
                ((ExtendedTextCell)bindable).ActivityIndicator.IsVisible = (bool)newValue;
            });

        public bool SpinnerActive
        {
            get { return (bool)GetValue(SpinnerActiveProperty); }
            set { SetValue(SpinnerActiveProperty, value); }
        }

        public ActivityIndicator ActivityIndicator { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty ImageSourceProperty =
            BindableProperty.Create(
                nameof(ImageSource),
                typeof(ImageSource),
                typeof(ExtendedTextCell),
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    ((ExtendedTextCell)bindable).CellImage.Source = newValue as ImageSource;
                    ((ExtendedTextCell)bindable).CellImage.IsVisible = true;
                }
            );

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        /***************************************************************************/

        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(
                nameof(Image),
                typeof(string),
                typeof(ExtendedTextCell),
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                    var current = (ExtendedTextCell)bindable;
                    current.CellImage.Source = Utility.GetImageFromAssets(newValue as string);
                    if(current.CellImage.Source != null)
                    {
                        current.CellImage.IsVisible = true;
                    }
                }
            );

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /***************************************************************************/

        public static readonly BindableProperty ImageSizeProperty = BindableProperty.Create(
            nameof(ImageSize),
            typeof(int),
            typeof(ExtendedTextCell),
            50,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
            ((ExtendedTextCell)bindable).CellImage.HeightRequest = (int)newValue;
            ((ExtendedTextCell)bindable).CellImage.WidthRequest = (int)newValue;
            });

        public int ImageSize
        {
            get { return (int)GetValue(ImageSizeProperty); }
            set
            {
                SetValue(ImageSizeProperty, value);
            }
        }

        /***************************************************************************/

        public static readonly BindableProperty SelectedProperty = BindableProperty.Create(
            nameof(Selected),
            typeof(bool),
            typeof(ExtendedTextCell),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                ((ExtendedTextCell)bindable).SelectedImage.Source = ImageSource.FromFile((bool)newValue ? "checked.png" : "checked_off.png");
            });

        public bool Selected
        {
            get { return (bool)GetValue(SelectedProperty); }
            set
            {
                SetValue(SelectedProperty, value);
            }
        }

        /***************************************************************************/

        public static readonly BindableProperty ShowCheckBoxProperty = BindableProperty.Create(
            nameof(ShowCheckBox),
            typeof(bool),
            typeof(ExtendedTextCell),
            false,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
            ((ExtendedTextCell)bindable).SelectedImage.IsVisible = (bool)newValue;
            });

        public bool ShowCheckBox
        {
            get { return (bool)GetValue(ShowCheckBoxProperty); }
            set
            {
                SetValue(ShowCheckBoxProperty, value);
            }
        }


        /***************************************************************************/

        public static readonly BindableProperty IsVisibleProperty = BindableProperty.Create(
            nameof(IsVisible),
            typeof(bool),
            typeof(ExtendedTextCell),
            true,
            propertyChanged: (bindable, oldValue, newValue) =>
            {
                if(!(bool)newValue)
                {
                    ((ExtendedTextCell)bindable).ViewLayout.HeightRequest = 0;
                    ((ExtendedTextCell)bindable).ViewLayout.Padding = 0;
                }
            });

        public bool IsVisible
        {
            get { return (bool)GetValue(IsVisibleProperty); }
            set
            {
                SetValue(IsVisibleProperty, value);
            }
        }

        public CircleImage CellImage { get; private set; }
        public TapGestureRecognizer GestureRecognizer { get; private set; }
        public Image SelectedImage { get; private set; }

        public ExtendedTextCell()
        {

            CellImage = new CircleImage
            {
                BorderColor = Color.White,
                BorderThickness = 0,
                Aspect = Aspect.AspectFill,
                WidthRequest = 50,
                HeightRequest= 50,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                IsVisible = false
            };
            ViewLayout.Children.Add(CellImage);

            var sublayout = new StackLayout { VerticalOptions = LayoutOptions.CenterAndExpand };
            TextLabel = new Label{ Margin = 0 };
            sublayout.Children.Add(TextLabel);

            DetailLabel = new Label { Margin = 0, IsVisible = false, FontSize = 11, TextColor = Utility.PRIMARY_COLOR };
            sublayout.Children.Add(DetailLabel);

            ViewLayout.Children.Add(sublayout);

            Text2Label = new Label { VerticalOptions = LayoutOptions.Center, 
                                     FontSize = 16,
                                     TextColor = Utility.PRIMARY_COLOR,
                                     HorizontalOptions = LayoutOptions.FillAndExpand,
                                     HorizontalTextAlignment = TextAlignment.End};
            ViewLayout.Children.Add(Text2Label);

            ActivityIndicator = new ActivityIndicator { 
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                IsRunning = true,
                IsVisible = false
            };
            ViewLayout.Children.Add(ActivityIndicator);

            SelectedImage = new Image
            {
                IsVisible = ShowCheckBox,
                Source = ImageSource.FromFile("checked_off.png"),
                Aspect = Aspect.AspectFit,
                WidthRequest = 30,
                HeightRequest = 30,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            ViewLayout.Children.Add(SelectedImage);

            GestureRecognizer = new TapGestureRecognizer();

            GestureRecognizer.Tapped += async (s, e) => {
                if (Command != null && Command.CanExecute(null))
                {
                    Command.Execute(null);
                } else
                {
                    await _navigator.PushAsync<InputTextViewModel>(
                        beforeNavigate: async (arg) =>
                        {
                            await arg.InitializeAsync(Text, Text2, (newVal) => { Text2 = newVal; });
                        });
                }
            };
            ViewLayout.GestureRecognizers.Add(GestureRecognizer);
            View = ViewLayout;
        }
    }
}
