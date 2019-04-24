using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using HuntLog.Controls;
using HuntLog.Helpers;
using HuntLog.InputViews;
using HuntLog.Services;
using ImageCircle.Forms.Plugin.Abstractions;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace HuntLog.Cells
{
    public class PickerCell : BaseCell
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(
                nameof(Text), 
                typeof(string), 
                typeof(PickerCell), 
                null,
                propertyChanged: (bindable, oldValue, newValue) => {
                ((PickerCell)bindable).TextLabel.Text = newValue as string;
                }
            );
        
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public Label TextLabel { get; private set; }

        /***************************************************************************/

        public static readonly BindableProperty PickerItemsProperty =
                    BindableProperty.Create(
                        nameof(PickerItems),
                        typeof(List<PickerItem>),
                        typeof(PickerCell),
                        null,
                        propertyChanged: (bindable, oldValue, newValue) => {
                            ((PickerCell)bindable).PickerItems = newValue as List<PickerItem>;
                            ((PickerCell)bindable).PopulatePickers();
                        }
                    );

        public List<PickerItem> PickerItems
        {
            get { return (List<PickerItem>)GetValue(PickerItemsProperty); }
            set { SetValue(PickerItemsProperty, value); }
        }
        public StackLayout PickersView { get; private set; }
        /***************************************************************************/

        public static readonly BindableProperty ModeProperty =
        BindableProperty.Create(
            nameof(Mode),
            typeof(PickerMode),
            typeof(PickerCell),
            PickerMode.Multiple,
            propertyChanged: (bindable, oldValue, newValue) => {
                ((PickerCell)bindable).Mode = (PickerMode) newValue;
            }
        );

        public PickerMode Mode
        {
            get { return (PickerMode)GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }

        /***************************************************************************/


        public PickerCell() : base()
        {
            var viewLayout = new Grid
            {
                Padding = new Thickness(10, 5),
                HeightRequest = 70
            };
            viewLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
            viewLayout.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            TextLabel = new Label {
                VerticalOptions = LayoutOptions.Center,
                LineBreakMode = LineBreakMode.NoWrap,
                WidthRequest = 90
            };

            PickersView = new StackLayout { 
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.EndAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            var scrollView = new ScrollView { 
                Orientation = ScrollOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.End
            };
            scrollView.Content = PickersView;

            viewLayout.Children.Add(TextLabel, 0, 0);
            viewLayout.Children.Add(scrollView, 1, 0);

            View = viewLayout;
        }

        private void PopulatePickers()
        {
            PickersView.Children.Clear();
            var size = 50;
            foreach(var item in PickerItems)
            {
                var wrap = new StackLayout {
                    HorizontalOptions = LayoutOptions.EndAndExpand,
                };
                if (Mode == PickerMode.Numeric) 
                {
                    wrap.Children.Add(new Button
                    {
                        Text = item.Custom && !item.Selected ? "..." : item.Title,
                        CornerRadius = item.Custom ? 0 : size / 2,
                        WidthRequest = size,
                        HeightRequest = size,
                        TextColor = Color.Black,
                        BorderColor = item.Selected ? Color.Black : Color.Gray,
                        BorderWidth = 2,
                        FontSize = 20,
                        BackgroundColor = item.Selected ? Color.Gold : Color.FromHex("EEE"),
                        HorizontalOptions = LayoutOptions.Center,
                        InputTransparent = true
                    });
                }
                else
                {
                    wrap.Children.Add(new CircleImage
                    {
                        Source = item.ImageSource,
                        BackgroundColor = Color.White,
                        HorizontalOptions = LayoutOptions.Center,
                        Aspect = Aspect.AspectFill,
                        BorderColor = item.Selected ? Color.Gold : Color.Gray,
                        BorderThickness = item.Selected ? 4 : 2,
                        WidthRequest = size,
                        HeightRequest = size,
                    });

                    wrap.Children.Add(new Label
                    {
                        Text = item.Title,
                        HorizontalOptions = LayoutOptions.Center,
                        FontSize = 12,
                        Margin = -6,
                        //BackgroundColor = item.Selected ? Color.Gold : Color.White
                    });
                }

                var gestureRecognizer = new TapGestureRecognizer();

                gestureRecognizer.Tapped += async (s, e) =>
                {
                    if(Mode == PickerMode.None) { return; }

                    await wrap.ScaleTo(0.75, 50, Easing.Linear);
                    await wrap.ScaleTo(1, 50, Easing.Linear);

                    item.Selected = !item.Selected;

                    if (item.Custom) 
                    {
                        await _navigator.PushAsync<InputTextViewModel>(
                            beforeNavigate: async (arg) =>
                            {
                                await arg.InitializeAsync("Velg antall", item.ID, (value) =>
                                {
                                    PickerItems.ForEach(p => p.Selected = false);
                                    item.Title = value;
                                    item.ID = value;
                                    item.Selected = true;
                                    
                                    PopulatePickers();
                                },
                                keyboard: Keyboard.Numeric);
                            });
                    }

                    if (Mode != PickerMode.Multiple) 
                    {
                        foreach(var i in PickerItems.Where(p => p.ID != item.ID)) 
                        {
                            i.Selected = false;
                        }
                    }

                    PopulatePickers();
                };

                wrap.GestureRecognizers.Add(gestureRecognizer);

                PickersView.Children.Add(wrap);
            }
        }

    }

    public enum PickerMode
    {
        Single,
        Multiple,
        Numeric,
        None
    }
}
